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
- Coinbase: https://advanced.coinbase.com/join/KTKSEBP * (10€ in BTC for you if you open an account)
- TradeRepublic: https://refnocode.trade.re/wfnk80zm * (10€ in stocks for you if you open an account)

(*) Affiliate link
Thank you very much for supporting me!

# Nuget
Get the latest version from nuget.org<br>
[![NuGet](https://img.shields.io/nuget/v/AppSettingsMaui.svg?style=flat-square&label=nuget)](https://www.nuget.org/packages/AppSettingsMaui/)
[![NuGet](https://img.shields.io/nuget/dt/AppSettingsMaui.svg)](https://www.nuget.org/packages/AppSettingsMaui)

# Usage

## JsonSerializerContext
See the `ExampleApp` project for an example of how to create a `JsonSerializerContext` and pass it to the `LoadSettings` and `SaveSettings` methods.
Add each object, which needs to be serialized/deserialized as `JsonSerializable` attribute.

```cs
    [JsonSerializable(typeof(Version))]
    [JsonSerializable(typeof(SettingsItem))]
    [JsonSourceGenerationOptions(WriteIndented = true)]
    public partial class AppSourceGenerationContext : JsonSerializerContext { }
```

Very important: Add each type which is used for your `SettingsApp` class here!

You also can set the `Context` globally by setting the property in the `AppHostBuilderExtensions.cs` file of your app:
```cs
    public static MauiAppBuilder ConfigureSettings(this MauiAppBuilder builder, string? hash = null, IDispatcher? dispatcher = null)
    {
        dispatcher ??= builder.Services.BuildServiceProvider().GetService<IDispatcher>();
        AppSettingsService settings = new()
        {
            Dispatcher = dispatcher,
            PassPhrase = hash,
            Context = AppSourceGenerationContext.Default
        };
        builder.Services.TryAddSingleton<IAppSettingsService>(settings);
        return builder;
    }
```

## Settings Object
In the .NET MAUI project, create a new `Class` in the `Services` folder (for instance `AppSettingsService.cs`) holding your setting properties.
Don't forget to also create an `Interface` for the service (for instance `IAppSettingsService.cs`) and add the properties there as well, 
so you can inject the service in your `ViewModels` and `App.xaml.cs` file.

```cs
public partial class AppSettingsService : MauiAppSettings<AppSettingsService>, IAppSettingsService
{
    
    #region Settings

    #region Version
    [ObservableProperty, MauiAppSetting(Name = nameof(App_SettingsVersion))]
    public partial Version App_SettingsVersion { get; set; } = new("1.0.0");

    #endregion

    #region CloudSync
    [ObservableProperty, MauiAppSetting(Name = nameof(Cloud_ShowInitialPrompt), DefaultValue = true)]
    public partial bool Cloud_ShowInitialPrompt { get; set; }

    [ObservableProperty, MauiAppSetting(Name = nameof(Cloud_ShowInitialPrompt), DefaultValue = SettingsStaticDefault.Cloud_EnableSync)]
    public partial bool Cloud_EnableSync { get; set; }
    #endregion

    #region Theme 
    [ObservableProperty, MauiAppSetting(Name = nameof(Theme_UseDeviceDefaultSettings), DefaultValue = SettingsStaticDefault.General_UseDeviceSettings)]
    public partial bool Theme_UseDeviceDefaultSettings { get; set; }

    [ObservableProperty, MauiAppSetting(Name = nameof(Theme_UseDarkTheme), DefaultValue = SettingsStaticDefault.General_UseDarkTheme)]
    public partial bool Theme_UseDarkTheme { get; set; }

    [ObservableProperty, MauiAppSetting(Name = nameof(Theme_PrimaryThemeColor), DefaultValue = SettingsStaticDefault.Theme_PrimaryThemeColor)]
    public partial string Theme_PrimaryThemeColor { get; set; }

    #endregion

    #region Localization

    [ObservableProperty, MauiAppSetting(Name = nameof(Localization_CultureCode), DefaultValue = SettingsStaticDefault.Localization_Default)]
    public partial string Localization_CultureCode { get; set; }

    #endregion

    #region Secure

    // Encrypt: The value is encrypt before saving it on the device, and decrypt when loaded
    // Note: Only `Secure` properties can be encrypted
    [ObservableProperty, MauiAppSetting(Name = nameof(Localization_CultureCode), DefaultValue ="", Secure = true, Encrypt = true)]
    public partial string User_Username { get; set; }

    // SkipForExport: Value is not added to the Dictionary when exporting the settings
    [ObservableProperty, MauiAppSetting(Name = nameof(Localization_CultureCode), DefaultValue ="", Secure = true, Encrypt = true, SkipForExport = true)]
    public partial string User_Password { get; set; }

    #endregion

    #endregion
}
```

## Dependency Injection
You can then inject the `IAppSettingsService` on each `ViewModel` or the `App.xaml.cs`, where needed.

```cs
    readonly IAppSettingsService? appSettings;
    public App(IServiceProvider serviceProvider, IAppSettingsService appSettings)
    {
        this.serviceProvider = serviceProvider;
        this.appSettings = appSettings;
        InitializeComponent();
    }
```

## Load
To load the settings from the storage, call `SettingsApp.LoadSettings()` (mostly in the `App` constructor of your App.xmls file. The project uses the `Maui.Storage.Preferences` in order to store the settings on the corresponding device.

```cs
// Load settings with the app context and a shared name (optional) ...
appSettings!.LoadSettings(AppSourceGenerationContext.Default, SharedName);
```

## Save
Whenever you do make changes to a settings property of your class, call `SettingsApp.SaveSettings()`. This will write the settings to the storage.
```cs
        [ObservableProperty]
        public partial bool AutoSyncOnStartup { get; set; } = true;
        partial void OnAutoSyncOnStartupChanged(bool value)
        {
            if (!IsLoading)
            {
                appSettings!.SevDesk_AutoSyncOnStartup = value;
                appSettings!.SaveSetting(setting => appSettings.SevDesk_AutoSyncOnStartup);
            }
        }
```

If you want to save a secure or/and encrypted property, you have to provide a key for encryption/decryption and use the `async` method:

```cs
        [ObservableProperty]
        public partial string AccessToken { get; set; } = string.Empty;
        partial void OnAccessTokenChanged(string value)
        {
            AccessTokenValid = !string.IsNullOrEmpty(value);
            if (!IsLoading && AccessTokenValid && value is not null)
            {
                appSettings!.SevDesk_AccessToken = value;
                appSettings!.SaveSettingAsync(
                    setting => appSettings.SevDesk_AccessToken,
                    appSettings.TryDecryptPassphrase(appSettings.Firebase_PassPhrase, appSettings.Encryption_Password)
                    );
            }
        }
```

# Encryption
With the service, you also can encrypt secure properties with a `AES` encryption. An example how it works is shown below.
Sample: https://github.com/AndreasReitberger/MauiSettings/tree/main/src/MauiSettings.Example

## App.xaml.cs
An example of how to load the settings from the `App.xaml`.

```cs
public partial class App : Application
{
    // Example key, it is recommended to generate the key on the device and save it as `Secure` property instead of
    // adding it in clear text to the source code.
    public static string Hash = "mYGUbR61NUNjIvdEv/veySPxQEWcCRUZ3SZ7TT72IuI=";
    
    readonly IAppSettingsService? appSettings;
    public App(IAppSettingsService appSettings)
    {
        this.appSettings = appSettings;
        InitializeComponent();

        // Example of how to generate a new key
        //string t = EncryptionManager.GenerateBase64Key();

        // Only Async methods do support encryption!
        _ = Task.Run(async () => await appSettings.LoadSettingsAsync(Hash));
        MainPage = new AppShell();
    }

    protected override void OnSleep()
    {
        base.OnSleep();
        if (appSettings?.SettingsChanged is true)
        {
            try
            {
                appSettings.SaveSettings(Hash);
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
    #region Fields
    readonly IAppSettingsService? appSettings;
    #endregion

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
            appSettings!.LicenseInfo = value;
            appSettings!.SaveSetting(setting => appSettings.LicenseInfo);
        }
    }

    [ObservableProperty]
    ObservableCollection<SettingsItem> settings = [];
    #endregion

    #region Ctor
    public MainPageViewModel(IAppSettingsService appSettings)
    {
        this.appSettings = appSettings;
        LoadSettings();
    }
    #endregion

    #region Methods
    void LoadSettings()
    {
        IsLoading = true;

        LicenseInfo = appSettings!.LicenseInfo;

        IsLoading = false;
    }
    #endregion

    #region Commands
    [RelayCommand]
    async Task SaveSettings() => await appSettings!.SaveSettingsAsync(key: App.Hash);

    [RelayCommand]
    async Task LoadSettingsFromDevice()
    {
        try
        {
            await appSettings!.LoadSettingsAsync(key: App.Hash);
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
        await appSettings!.ExhangeKeyAsync(oldKey: App.Hash, newKey: newKey);
        App.Hash = HashKey = newKey;
        LoadSettings();
    }

    [RelayCommand]
    async Task ToDictionary()
    {
        // All "SkipForExport" should be missing here.
        Dictionary<string, Tuple<object?, Type>> dict = await appSettings.ToDictionaryAsync();
        Settings = [.. dict.Select(kp => new SettingsItem() { Key = kp.Key, Value = kp.Value.Item1.ToString() })];
    }
    #endregion
}
```
For more information, please see the example project.
