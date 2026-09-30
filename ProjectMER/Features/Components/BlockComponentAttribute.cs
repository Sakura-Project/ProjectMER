using ProjectMER.Features.Enums;

namespace ProjectMER.Features.Components;

[AttributeUsage(AttributeTargets.Class)]
public sealed class BlockComponentAttribute : Attribute
{
    public ComponentType Type { get; }
    
    public BlockComponentAttribute(ComponentType type)
    {
        Type = type;
    }
}