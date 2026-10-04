using Sakura.API.ServerSpecific;

namespace ProjectMER.Settings;

internal static class SettingsRegistry
{
    public static CustomHeader ProjectMerHeader { get; } = new("ProjectMER");
    public static SchematicName SchematicName { get; } = new SchematicName();
}