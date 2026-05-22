using AndreasReitberger.Maui.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AndreasReitberger.Maui
{
    // All the code in this file is only included on Mac Catalyst.
    public partial class MauiAppSettingsService<SO> : ObservableObject, IMauiAppSettingsService<SO> where SO : new()
    {

    }
}