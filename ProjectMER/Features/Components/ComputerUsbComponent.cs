using LabApi.Features.Wrappers;
using ProjectMER.Features.Enums;
using UnityEngine;

namespace ProjectMER.Features.Components;

[BlockComponent(ComponentType.ComputerUsb)]
public sealed class ComputerUsbComponent : MonoBehaviour, IBlockComponent
{
    public Room? Room { get; private set; }
    
    public ComponentData Compile()
    {
        return new ComponentData()
        {
            Type = ComponentType.ComputerUsb,
        };
    }

    public void Decompile(ComponentData componentData)
    {
        
    }

    private void Start()
    {
        Room = Room.GetRoomAtPosition(transform.position);
    }
}