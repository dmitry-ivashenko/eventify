#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Eventify.Core.Runtime;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Eventify.Core.Editor
{
    public class StreamsDebugger : EditorWindow
    {
        private const string PREFS = "StreamsDebugger.searchText";
        private SearchField _searchField;
        private string _searchText;
        private bool _sortByTime;
        private CallInfoBase _selected;
        private GUIStyle _textareaStyle;
        
        private Rect _upperPanel;
        private Rect lowerPanel;
        private Rect resizer;
        private Rect menuBar;

        private float sizeRatio = 0.5f;
        private bool isResizing;

        private float resizerHeight = 5f;
        private float menuBarHeight = 20f;

        private Vector2 _upperPanelScroll;
        private Vector2 lowerPanelScroll;

        private GUIStyle resizerStyle;
        private GUIStyle boxStyle;
        private GUIStyle textAreaStyle;

        private Texture2D boxBgOdd;
        private Texture2D boxBgEven;
        private Texture2D boxBgSelected;
        private int _maskUi = -1;
        private int _maskDebug = -1;
        private int _maskScene = -1;
        private int _maskServer = -1;
        private int _maskSync = -1;
        private int _maskNetMaster = -1;
        private int _maskNetOpponent = -1;
        private int _maskNetGlobal = -1;
        private int _maskNetPlayer = -1;
        private int _maskSound = -1;
        private int _maskBattleSound = -1;
        private int _maskProfiler = -1;
        
        private static readonly Dictionary<string, UiEvent> _dictUi = typeof(UiEvent).GetEnumValues().Convert(o => (UiEvent) o).ToDictionary(e => e.ToString(), e => e);
        private static readonly Dictionary<string, DebugEvent> _dictDebug = typeof(DebugEvent).GetEnumValues().Convert(o => (DebugEvent) o).ToDictionary(e => e.ToString(), e => e);
        private static readonly Dictionary<string, SceneEvent> _dictScene = typeof(SceneEvent).GetEnumValues().Convert(o => (SceneEvent) o).ToDictionary(e => e.ToString(), e => e);
        private static readonly Dictionary<string, ServerEvent> _dictServer = typeof(ServerEvent).GetEnumValues().Convert(o => (ServerEvent) o).ToDictionary(e => e.ToString(), e => e);
        private static readonly Dictionary<string, SyncEvent> _dictSync = typeof(SyncEvent).GetEnumValues().Convert(o => (SyncEvent) o).ToDictionary(e => e.ToString(), e => e);
        private static readonly Dictionary<string, NetMasterEvent> _dictNetMaster = typeof(NetMasterEvent).GetEnumValues().Convert(o => (NetMasterEvent) o).ToDictionary(e => e.ToString(), e => e);
        private static readonly Dictionary<string, NetOpponentEvent> _dictNetOpponent = typeof(NetOpponentEvent).GetEnumValues().Convert(o => (NetOpponentEvent) o).ToDictionary(e => e.ToString(), e => e);
        private static readonly Dictionary<string, NetGlobalEvent> _dictNetGlobal = typeof(NetGlobalEvent).GetEnumValues().Convert(o => (NetGlobalEvent) o).ToDictionary(e => e.ToString(), e => e);
        private static readonly Dictionary<string, NetPlayerEvent> _dictNetPlayer = typeof(NetPlayerEvent).GetEnumValues().Convert(o => (NetPlayerEvent) o).ToDictionary(e => e.ToString(), e => e);
        private static readonly Dictionary<string, SoundEvent> _dictSound = typeof(SoundEvent).GetEnumValues().Convert(o => (SoundEvent) o).ToDictionary(e => e.ToString(), e => e);
        private static readonly Dictionary<string, ProfilerEvent> _dictProfiler = typeof(ProfilerEvent).GetEnumValues().Convert(o => (ProfilerEvent) o).ToDictionary(e => e.ToString(), e => e);
        
        private readonly List<UiEvent> _selectedUi = new List<UiEvent>();
        private readonly List<DebugEvent> _selectedDebug = new List<DebugEvent>();
        private readonly List<SceneEvent> _selectedScene = new List<SceneEvent>();
        private readonly List<ServerEvent> _selectedServer = new List<ServerEvent>();
        private readonly List<SyncEvent> _selectedSync = new List<SyncEvent>();
        private readonly List<NetMasterEvent> _selectedMaster = new List<NetMasterEvent>();
        private readonly List<NetOpponentEvent> _selectedOpponent = new List<NetOpponentEvent>();
        private readonly List<NetGlobalEvent> _selectedGlobal = new List<NetGlobalEvent>();
        private readonly List<NetPlayerEvent> _selectedPlayer = new List<NetPlayerEvent>();
        private readonly List<SoundEvent> _selectedSound = new List<SoundEvent>();
        private readonly List<ProfilerEvent> _selectedProfiler = new List<ProfilerEvent>();
        
        private readonly Dictionary<string, double> _grouped = new Dictionary<string, double>();
        private bool _showFilters;

        [MenuItem("Tools/Streams Debugger")]
        private static void OpenWindow()
        {
            var window = GetWindow<StreamsDebugger>("Streams");
            window.position = new Rect(Screen.width / 2f, Screen.height / 2f, 800, 500);
            window.titleContent = new GUIContent("Streams");
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Streams");

            resizerStyle = new GUIStyle();
            resizerStyle.normal.background = EditorGUIUtility.Load("icons/d_AvatarBlendBackground.png") as Texture2D;

            boxStyle = new GUIStyle();
            boxStyle.normal.textColor = new Color(0.19f, 0.19f, 0.19f, 1f);

            boxBgOdd = EditorGUIUtility.Load("builtin skins/lightskin/images/cn entrybackodd.png") as Texture2D;
            boxBgEven = EditorGUIUtility.Load("builtin skins/lightskin/images/cnentrybackeven.png") as Texture2D;
            boxBgSelected = EditorGUIUtility.Load("builtin skins/lightskin/images/menuitemhover.png") as Texture2D;

            textAreaStyle = new GUIStyle();
            textAreaStyle.normal.textColor = new Color(0.9f, 0.9f, 0.9f);
            textAreaStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/projectbrowsericonareabg.png") as Texture2D;

            _searchField = new SearchField();
            _searchField.SetFocus();
            _searchText = EditorPrefs.GetString(PREFS);

            try
            {
                _textareaStyle = new GUIStyle(EditorStyles.textArea)
                {
                    fontStyle = FontStyle.Normal,
                    normal = {textColor = Color.black, background = Texture2D.blackTexture},
                    onNormal = {textColor = Color.black, background = Texture2D.blackTexture},
                    hover = {textColor = Color.black, background = Texture2D.blackTexture},
                    onHover = {textColor = Color.black, background = Texture2D.blackTexture},
                    focused = {textColor = Color.black, background = Texture2D.blackTexture},
                    onFocused = {textColor = Color.black, background = Texture2D.blackTexture},
                    active = {textColor = Color.black, background = Texture2D.blackTexture},
                    onActive = {textColor = Color.black, background = Texture2D.blackTexture},
                };
            }
            catch (Exception)
            {
                // ignored
            }
            
            UpdateSelectedEnums();
        }


        private void SaveSearchText()
        {
            EditorPrefs.SetString(PREFS, _searchText);
        }

        private bool Filtered(string text, CallInfoBase item)
        {
            var containEnums = item.IsOneOf(_selectedUi)
                               || item.IsOneOf(_selectedDebug)
                               || item.IsOneOf(_selectedScene)
                               || item.IsOneOf(_selectedServer)
                               || item.IsOneOf(_selectedSync)
                               || item.IsOneOf(_selectedMaster)
                               || item.IsOneOf(_selectedOpponent)
                               || item.IsOneOf(_selectedGlobal)
                               || item.IsOneOf(_selectedPlayer)
                               || item.IsOneOf(_selectedSound)
                               || item.IsOneOf(_selectedProfiler)
                               ;
            
            return containEnums && (_searchText.IsEmpty() || text.ContainsIgnoreCase(_searchText));
        }

        private void OnGUI()
        {
            DrawPopup();
            DrawMenuBar();
            DrawUpperPanel();
            DrawLowerPanel();
            DrawResizer();
            DrawPopup();

            ProcessEvents(Event.current);

            if (GUI.changed) Repaint();
        }

        private void DrawPopup()
        {
            if (!_showFilters) return;
            
            const int width = 300;
            const int height = 500;
            GUILayout.BeginArea(new Rect(menuBar.xMax - width, menuBar.height, width, height));    
            GUILayout.BeginVertical("box");
            {
                var prevSum = _maskUi + _maskScene + _maskServer + _maskSync + _maskNetMaster + _maskNetOpponent + _maskNetGlobal + _maskNetPlayer;
                    
                _maskUi = EditorGUILayout.MaskField("Ui", _maskUi, _dictUi.Keys.ToArray());
                _maskDebug = EditorGUILayout.MaskField("Debug", _maskDebug, _dictDebug.Keys.ToArray());
                _maskScene = EditorGUILayout.MaskField("Scene", _maskScene, _dictScene.Keys.ToArray());
                _maskServer = EditorGUILayout.MaskField("Server", _maskServer, _dictServer.Keys.ToArray());
                _maskSync = EditorGUILayout.MaskField("Sync", _maskSync, _dictSync.Keys.ToArray());
                _maskNetMaster = EditorGUILayout.MaskField("NetMaster", _maskNetMaster, _dictNetMaster.Keys.ToArray());
                _maskNetOpponent = EditorGUILayout.MaskField("NetOpponent", _maskNetOpponent, _dictNetOpponent.Keys.ToArray());
                _maskNetGlobal = EditorGUILayout.MaskField("NetGlobal", _maskNetGlobal, _dictNetGlobal.Keys.ToArray());
                _maskNetPlayer = EditorGUILayout.MaskField("NetPlayer", _maskNetPlayer, _dictNetPlayer.Keys.ToArray());
                _maskSound = EditorGUILayout.MaskField("Sound", _maskSound, _dictSound.Keys.ToArray());
                _maskProfiler = EditorGUILayout.MaskField("Profiler", _maskProfiler, _dictProfiler.Keys.ToArray());

                var newSum = _maskUi 
                             + _maskDebug 
                             + _maskScene 
                             + _maskServer 
                             + _maskSync 
                             + _maskNetMaster 
                             + _maskNetOpponent 
                             + _maskNetGlobal 
                             + _maskNetPlayer 
                             + _maskSound 
                             + _maskBattleSound
                             + _maskProfiler 
                             ;

                if (newSum != prevSum)
                {
                    UpdateSelectedEnums();
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void DrawMenuBar()
        {
            menuBar = new Rect(0, 0, position.width, menuBarHeight);

            GUILayout.BeginArea(menuBar, EditorStyles.toolbar);
            GUILayout.BeginHorizontal();

            GUILayout.BeginHorizontal(EditorStyles.toolbar);
            {
                if (GUILayout.Button("Clear", EditorStyles.toolbarButton))
                {
                    CallInfoBase.FullLog.Clear();
                }

                _sortByTime = GUILayout.Toggle(_sortByTime, "Collapse", EditorStyles.toolbarButton);
                
                GUILayout.Space(10);
                GUILayout.FlexibleSpace();
                
                var prevValue = _searchText;
                _searchText = _searchField.OnGUI(_searchText) ?? "";

                if (prevValue != _searchText)
                {
                    SaveSearchText();
                }

                var fixedWidth = EditorStyles.toolbarDropDown.fixedWidth;
                EditorStyles.toolbarDropDown.fixedWidth = 50;
                _showFilters = EditorGUILayout.Foldout(_showFilters, new GUIContent("Filter"), true, EditorStyles.toolbarDropDown);
                EditorStyles.toolbarDropDown.fixedWidth = fixedWidth;
            }
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void UpdateSelectedEnums()
        {
            _selectedUi.Clear();
            _selectedDebug.Clear();
            _selectedScene.Clear();
            _selectedServer.Clear();
            _selectedSync.Clear();
            _selectedMaster.Clear();
            _selectedOpponent.Clear();
            _selectedGlobal.Clear();
            _selectedPlayer.Clear();
            _selectedSound.Clear();
            _selectedProfiler.Clear();

            for (var i = 0; i < _dictUi.Values.Count; i++) if ((_maskUi & 1 << i) != 0) _selectedUi.Add(_dictUi.Values.ElementAt(i));
            for (var i = 0; i < _dictDebug.Values.Count; i++) if ((_maskDebug & 1 << i) != 0) _selectedDebug.Add(_dictDebug.Values.ElementAt(i));
            for (var i = 0; i < _dictScene.Values.Count; i++) if ((_maskScene & 1 << i) != 0) _selectedScene.Add(_dictScene.Values.ElementAt(i));
            for (var i = 0; i < _dictServer.Values.Count; i++) if ((_maskServer & 1 << i) != 0) _selectedServer.Add(_dictServer.Values.ElementAt(i));
            for (var i = 0; i < _dictSync.Values.Count; i++) if ((_maskSync & 1 << i) != 0) _selectedSync.Add(_dictSync.Values.ElementAt(i));
            for (var i = 0; i < _dictNetMaster.Values.Count; i++) if ((_maskNetMaster & 1 << i) != 0) _selectedMaster.Add(_dictNetMaster.Values.ElementAt(i));
            for (var i = 0; i < _dictNetOpponent.Values.Count; i++) if ((_maskNetOpponent & 1 << i) != 0) _selectedOpponent.Add(_dictNetOpponent.Values.ElementAt(i));
            for (var i = 0; i < _dictNetGlobal.Values.Count; i++) if ((_maskNetGlobal & 1 << i) != 0) _selectedGlobal.Add(_dictNetGlobal.Values.ElementAt(i));
            for (var i = 0; i < _dictNetPlayer.Values.Count; i++) if ((_maskNetPlayer & 1 << i) != 0) _selectedPlayer.Add(_dictNetPlayer.Values.ElementAt(i));
            for (var i = 0; i < _dictSound.Values.Count; i++) if ((_maskSound & 1 << i) != 0) _selectedSound.Add(_dictSound.Values.ElementAt(i));
            for (var i = 0; i < _dictProfiler.Values.Count; i++) if ((_maskProfiler & 1 << i) != 0) _selectedProfiler.Add(_dictProfiler.Values.ElementAt(i));
        }

        private void DrawUpperPanel()
        {
            _upperPanel = new Rect(0, menuBarHeight, position.width, (position.height * sizeRatio) - menuBarHeight);

            GUILayout.BeginArea(_upperPanel);
            {
                _upperPanelScroll = GUILayout.BeginScrollView(_upperPanelScroll);
                {
                    var lineNumber = 0;
                    var items = CallInfoBase.FullLog.Queue;

                    GUILayout.BeginVertical(GUILayout.ExpandHeight(true));
                    {
                        if (_sortByTime)
                        {
                            _grouped.Clear();
                            foreach (var item in items)
                            {
                                var key = item.GetTitle();
                                if (!Filtered(key, item)) continue;

                                if (_grouped.TryGetValue(key, out var duration))
                                {
                                    _grouped[key] = duration + item.Duration.TotalMilliseconds;
                                }
                                else
                                {
                                    _grouped[key] = item.Duration.TotalMilliseconds;
                                }
                            }

                            foreach (var pair in _grouped.OrderByDescending(pair => pair.Value))
                            {
                                var line = $"[{pair.Value:F5} ms] {pair.Key}";
                                if (DrawBox(line, LogType.Log, ++lineNumber % 2 == 0, false))
                                {

                                }
                            }
                        }
                        else
                        {
                            foreach (var item in items)
                            {
                                var line = item.SingleLine();
                                if (!item.IsPerformed || !Filtered(line, item)) continue;

                                if (DrawBox(line, LogType.Log, ++lineNumber % 2 == 0, _selected == item))
                                {
                                    _selected = item;
                                    GUI.changed = true;
                                }
                            }
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();
        }

        private void DrawLowerPanel()
        {
            lowerPanel = new Rect(0, (position.height * sizeRatio) + resizerHeight, position.width,
                (position.height * (1 - sizeRatio)) - resizerHeight);

            GUILayout.BeginArea(lowerPanel);
            lowerPanelScroll = GUILayout.BeginScrollView(lowerPanelScroll);

            if (_selected != null)
            {
                var text = $"[{_selected.Time.ToLongTimeString()}] " +
                           $"{_selected.GetTitle()}" +
                           $" - {_selected.Duration.TotalMilliseconds} ms\n{_selected.Log}";

                GUILayout.TextArea(text, _textareaStyle);
            }

            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void DrawResizer()
        {
            resizer = new Rect(0, (position.height * sizeRatio) - resizerHeight, position.width, resizerHeight * 2);

            GUILayout.BeginArea(
                new Rect(resizer.position + (Vector2.up * resizerHeight), new Vector2(position.width, 2)),
                resizerStyle);
            GUILayout.EndArea();

            EditorGUIUtility.AddCursorRect(resizer, MouseCursor.ResizeVertical);
        }

        private bool DrawBox(string content, LogType boxType, bool isOdd, bool isSelected)
        {
            if (isSelected)
            {
                boxStyle.normal.background = boxBgSelected;
            }
            else
            {
                boxStyle.normal.background = isOdd ? boxBgOdd : boxBgEven;
            }

            switch (boxType)
            {
                case LogType.Error:
                    break;
                case LogType.Exception:
                    break;
                case LogType.Assert:
                    break;
                case LogType.Warning:
                    break;
                case LogType.Log:
                    break;
            }

            var value = GUILayout.Button(new GUIContent(content), boxStyle, GUILayout.ExpandWidth(true), GUILayout.Height(18));
            
            if (value)
            {
                _showFilters = false;
            }
            
            return value;
        }

        private void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0 && resizer.Contains(e.mousePosition))
                    {
                        isResizing = true;
                    }

                    break;

                case EventType.MouseUp:
                    isResizing = false;
                    break;
            }

            Resize(e);
        }

        private void Resize(Event e)
        {
            if (isResizing)
            {
                sizeRatio = e.mousePosition.y / position.height;
                Repaint();
            }
        }
        
        private void OnSelectionChange()
        {
            Repaint();
        }

        private void OnInspectorUpdate()
        {
            Repaint();
        }
    }

    public static class Extensions
    {
        public static IEnumerable<T> Convert<T>(this IEnumerable source, Func<object, T> converter)
        {
            foreach (object obj in source)
            {
                yield return converter(obj);
            }
        }
        
        public static bool IsEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }
    }
}

#endif
