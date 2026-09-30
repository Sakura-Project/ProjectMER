using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using ProjectMER.Features.Enums;
using UnityEngine;

namespace ProjectMER.Features.Components;

public static class ComponentManager
{
    private static readonly Dictionary<ComponentType, Type> BlockComponents = new();

    public static void RegisterAll()
    {
        BlockComponents.Clear();

        var componentTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass 
                        && !t.IsAbstract 
                        && typeof(IBlockComponent).IsAssignableFrom(t)
                        && typeof(MonoBehaviour).IsAssignableFrom(t));

        foreach (var type in componentTypes)
        {
            var attr =  type.GetCustomAttribute<BlockComponentAttribute>();
            if (attr == null)
            {
                continue;
            }
            
            if (!BlockComponents.TryAdd(attr.Type, type))
                continue;
        }
    }

    public static bool TryAddComponent(GameObject go, ComponentType type, [NotNullWhen(true)] out IBlockComponent? component)
    {
        component = null;
        if (go == null)
        {
            Logger.Warn("Failed create component: GameObject is null.");
            return false;
        }

        if (!BlockComponents.TryGetValue(type, out var componentType))
        {
            Logger.Warn("Failed create component: Type not found.");
            return false;
        }

        component = go.AddComponent(componentType) as IBlockComponent;
        if (component == null)
        {
            Logger.Warn("Failed create component: unity not created component.");
            return false;
        }
        return true;
    }

    public static void AssignComponents(GameObject go, List<ComponentData> data)
    {
        foreach (var componentData in data)
        {
            if (TryAddComponent(go, componentData.Type, out var component))
            {
                component.Decompile(componentData);
            }
        }
    }
}