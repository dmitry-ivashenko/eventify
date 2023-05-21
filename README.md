
# Enum-Based Event Bus For Unity

Example of implementation Event Bus system for Unity, allowing different parts of your application to communicate with each other through events. The Event Bus is based on Enums, which makes it easy to define and manage events in your code.


[![MIT License](https://img.shields.io/badge/License-MIT-green.svg)](https://choosealicense.com/licenses/mit/)


## Installation

- Install the UniRx library by importing the unitypackage file into your project or through the Unity Package Manager by adding to your manifest.json:

```bash
{
  "dependencies": {
    "com.neuecc.unirx": "7.1.0"
  },
  "scopedRegistries": [
    {
      "name": "package.openupm.com",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.neuecc.unirx"
      ]
    }
  ]
}

```

- Import the eventify.unitypackage file into your Unity project by selecting "Assets" > "Import Package" > "Custom Package" from the menu.

## Examples

Raising an event using the extension method ```Raise``` with and without arguments:
```cs
// Raising simple event without params
UiEvent.BuyButtonClicked.Raise();

// Raising event with data
UiEvent.MouseClick.Raise<Vector2>(new Vector2())

// Raising network event through Photon Unity Networking
NetEvent.PlayerReady.Raise(_playerId);
```

Subscribing and unsubscribing to events from different event buses with different arguments:

```cs
_disposable = new CompositeDisposable
{
    // Subscribing to all different event buses in the same way
    UiEvent.ExitBattleClick.Subscribe(OnExitBattleClick),   
    BackendEvent.BattleReset.Subscribe(OnBattleReset),
    DelayedEvent.ChangeWindDirection.Subscribe<Direction>(OnChangeWindDirection),
    NetEvent.PlayerReady.Subscribe<PlayerReadyData>(OnPlayerReady),
    DebugEvent.KillOpponentUnits.Subscribe(OnKillOpponentUnits),
    EditorEvent.SelectScene.Subscribe<string>(OnEditorSelectScene),
};

// Unsubscribe
_disposable.Dispose();
```


Here's how to filter events from the bus based on the parameters passed:
```cs
_disposable = new CompositeDisposable
{
    // Subscribe to the SetSoundVolume event with any value
    UiEvent.SetSoundVolume.Subscribe<float>(OnSetVolume),

    // Subscribe to the SetSoundVolume event
    // and call the OnMute method when it is raised with a value of 0f
    UiEvent.SetSoundVolume.Where<float>(x => x == 0f).Subscribe(OnMute),
};
```

## License

[MIT](https://choosealicense.com/licenses/mit/)


![Logo](https://github.com/dmitry-ivashenko/eventify/blob/main/evetify.png?raw=true)

