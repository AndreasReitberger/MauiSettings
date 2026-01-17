using MauiSettings.Example.Models.Settings;
using System.Text.Json.Serialization;

namespace MauiSettings.Example.SourceGeneration
{
    [JsonSerializable(typeof(SettingsItem))]
    [JsonSourceGenerationOptions(WriteIndented = true)]
    public partial class AppSourceGenerationContext : JsonSerializerContext { }
}
