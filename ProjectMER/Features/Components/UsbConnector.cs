using LabApi.Features.Wrappers;
using ProjectMER.Features.Enums;
using Sakura.API;
using Sakura.CustomItems.Extensions;
using Sakura.CustomItems.Items.UsbDrive;
using UnityEngine;

namespace ProjectMER.Features.Components;

// [BlockComponent(ComponentType.UsbConnector)]
public sealed class UsbConnector : MonoBehaviour, IBlockComponent, IInteractableObject
{
    public Room? Room { get; private set; }
    
    public ComponentData Compile()
    {
        return new ComponentData()
        {
            Type = ComponentType.UsbConnector,
            Properties = new Dictionary<string, object>()
        };
    }

    public void Decompile(ComponentData componentData)
    {
        
    }

    public bool CanInteract(Player player)
    {
        if (player.IsDestroyed || player.CurrentItem == null)
            return false;
        if (!player.CurrentItem.TryGetCustomItem(out var item) || item is not UsbDrive usbDrive)
        {
            return false;
        }

        return true;
    }

    public void Interact(Player player)
    {
        
    }

    private void Awake()
    {
        Room = Room.GetRoomAtPosition(transform.position);
    }
}