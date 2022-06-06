# MauiSettings
A nuget to improve settings storage (locally and eventually in the cloud) on .NET MAUI projects.

# Usage
## Settings Object
In the .NET MAUI project, create a new `Class` (for instance `SettingsApp.cs`) holding your setting properties.

```cs
public partial class SettingsApp : MauiSettings<SettingsApp>
{
    
    #region Settings

    #region Version
    [MauiSetting(Name = nameof(App_SettingsVersion))]
    public static Version App_SettingsVersion { get; set; } = new("1.0.0");

    #endregion

    #region CloudSync
    [MauiSetting(Name = nameof(Cloud_ShowInitialPrompt), DefaultValue = true)]
    public static bool Cloud_ShowInitialPrompt { get; set; }

    [MauiSetting(Name = nameof(Cloud_ShowInitialPrompt), DefaultValue = SettingsStaticDefault.Cloud_EnableSync)]
    public static bool Cloud_EnableSync { get; set; }
    #endregion

    #region Theme 
    [MauiSetting(Name = nameof(Theme_UseDeviceDefaultSettings), DefaultValue = SettingsStaticDefault.General_UseDeviceSettings)]
    public static bool Theme_UseDeviceDefaultSettings { get; set; }

    [MauiSetting(Name = nameof(Theme_UseDarkTheme), DefaultValue = SettingsStaticDefault.General_UseDarkTheme)]
    public static bool Theme_UseDarkTheme { get; set; }

    [MauiSetting(Name = nameof(Theme_PrimaryThemeColor), DefaultValue = SettingsStaticDefault.Theme_PrimaryThemeColor)]
    public static string Theme_PrimaryThemeColor { get; set; }

    #endregion

    #region Localization

    [MauiSetting(Name = nameof(Localization_CultureCode), DefaultValue = SettingsStaticDefault.Localization_Default)]
    public static string Localization_CultureCode { get; set; }

    #endregion

    #endregion
}
```

## Load
To load the settings from the storage, call `SettingsApp.LoadSettings()` (mostly in the `App` constructor of your App.xmls file. The project uses the `Maui.Storage.Preferences` in order to store the settings on the corresponding device.

## Save
Whenever you do make changes to a settings property of your class, call `SettingsApp.SaveSettings()`. This will write the settings to the storage.
