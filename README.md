# MauiSettings
A nuget to improve settings storage (locally and eventually in the cloud) on .NET MAUI projects.

The plugin idea is based on the Advexp.Settings.Local nuget by Alexey Ivakin</br>
Repo: https://bitbucket.org/advexp/component-advexp.settings/src/master/</br>
License: Apache-2.0 (https://licenses.nuget.org/Apache-2.0)</br>

This project was created from scratch, however uses the basic idea to keep all Settings in the
static object. All taken and changed files have been marked so.

# Support me
If you want to support me, you can order over following affilate links (I'll get a small share from your purchase from the corresponding store).

- Prusa: http://www.prusa3d.com/#a_aid=AndreasReitberger *
- Jake3D: https://tidd.ly/3x9JOBp * 
- Amazon: https://amzn.to/2Z8PrDu *
- Coinbase: https://advanced.coinbase.com/join/KTKSEBP * (Open an account)

(*) Affiliate link
Thank you very much for supporting me!

# Nuget
Get the latest version from nuget.org<br>
[![NuGet](https://img.shields.io/nuget/v/SettingsMaui.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/SettingsMaui/)
[![NuGet](https://img.shields.io/nuget/dt/SettingsMaui.svg)](https://www.nuget.org/packages/SettingsMaui)

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

    #region Secure

    // Encrypt: The value is encrypt before saving it on the device, and decrypt when loaded
    // Note: Only `Secure` properties can be encrypted
    [MauiSetting(Name = nameof(Localization_CultureCode), DefaultValue ="", Secure = true, Encrypt = true)]
    public static string User_Username { get; set; }

    // SkipForExport: Value is not added to the Dictionary when exporting the settings
    [MauiSetting(Name = nameof(Localization_CultureCode), DefaultValue ="", Secure = true, Encrypt = true, SkipForExport = true)]
    public static string User_Password { get; set; }

    #endregion

    #endregion
}
```

## Load
To load the settings from the storage, call `SettingsApp.LoadSettings()` (mostly in the `App` constructor of your App.xmls file. The project uses the `Maui.Storage.Preferences` in order to store the settings on the corresponding device.

## Save
Whenever you do make changes to a settings property of your class, call `SettingsApp.SaveSettings()`. This will write the settings to the storage.

# Encryption
Starting from version `1.0.6`, you can encrypt secure properties with a `AES`encryption. An example how it works is shown below.
Sample: https://github.com/AndreasReitberger/MauiSettings/tree/main/src/MauiSettings.Example

## App.xaml.cs
An example of how to load the settings from the `App.xaml`.

```cs
public partial class App : Application
{
    // Example key, it is recommended to generate the key on the device and save it as `Secure` property instead of
    // adding it in clear text to the source code.
    public static string Hash = "mYGUbR61NUNjIvdEv/veySPxQEWcCRUZ3SZ7TT72IuI=";
    public App()
    {
        InitializeComponent();

        // Example of how to generate a new key
        //string t = EncryptionManager.GenerateBase64Key();

        // Only Async methods do support encryption!
        _ = Task.Run(async () => await SettingsApp.LoadSettingsAsync(Hash));
        MainPage = new AppShell();
    }

    protected override void OnSleep()
    {
        base.OnSleep();
        if (SettingsApp.SettingsChanged)
        {
            try
            {
                SettingsApp.SaveSettings(Hash);
            }
            catch (Exception)
            {

            }
        }
    }
}
```

## MainPageViewModel
An example of a`ViewModel` loading and saving encrypted settings.
```cs
public partial class MainPageViewModel : ObservableObject
{
    #region Settings
    [ObservableProperty]
    bool isLoading = false;

    [ObservableProperty]
    string hashKey = App.Hash;

    [ObservableProperty]
    string licenseInfo = string.Empty;
    partial void OnLicenseInfoChanged(string value)
    {
        if (!IsLoading)
        {
            SettingsApp.LicenseInfo = value;
            SettingsApp.SettingsChanged = true;
        }
    }

    [ObservableProperty]
    ObservableCollection<SettingsItem> settings = [];
    #endregion

    #region Ctor
    public MainPageViewModel()
    {
        LoadSettings();
    }
    #endregion

    #region Methods
    void LoadSettings()
    {
        IsLoading = true;

        LicenseInfo = SettingsApp.LicenseInfo;

        IsLoading = false;
    }
    #endregion

    #region Commands
    [RelayCommand]
    async Task SaveSettings() => await SettingsApp.SaveSettingsAsync(key: App.Hash);

    [RelayCommand]
    async Task LoadSettingsFromDevice()
    {
        try
        {
            await SettingsApp.LoadSettingsAsync(key: App.Hash);
            LoadSettings();
        }
        catch(Exception)
        {
            // Throus if the key missmatches
        }
    }

    [RelayCommand]
    async Task ExchangeHashKey()
    {
        string newKey = EncryptionManager.GenerateBase64Key();
        await SettingsApp.ExhangeKeyAsync(oldKey: App.Hash, newKey: newKey);
        App.Hash = HashKey = newKey;
        LoadSettings();
    }

    [RelayCommand]
    async Task ToDictionary()
    {
        // All "SkipForExport" should be missing here.
        Dictionary<string, Tuple<object, Type>> dict = await SettingsApp.ToDictionaryAsync();
        Settings = [.. dict.Select(kp => new SettingsItem() { Key = kp.Key, Value = kp.Value.Item1.ToString() })];
    }
    #endregion
}
```
For more information, please see the example project.
