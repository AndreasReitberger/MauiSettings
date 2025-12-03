using AndreasReitberger.Maui.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AndreasReitberger.Maui
{
    // All the code in this file is only included on Android.
    public partial class MauiSettingsGeneric<SO> : ObservableObject, IMauiSettingsGeneric<SO> where SO : new()
    {
    }
}