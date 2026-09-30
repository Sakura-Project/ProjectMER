using ProjectMER.Features.Enums;

namespace ProjectMER.Features.Components;

[Serializable]
public sealed class ComponentData
{
    public ComponentType Type { get; set; }
    public Dictionary<string, object> Properties { get; set; } = new();
}