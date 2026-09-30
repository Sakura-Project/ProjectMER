using CommandSystem.Commands.RemoteAdmin;
using Interactables;
using Interactables.Interobjects.DoorUtils;
using LabApi.Features.Wrappers;
using Newtonsoft.Json.Linq;
using PlayerRoles;
using ProjectMER.Features.Enums;
using Sakura.API;
using Sakura.CustomItems.Extensions;
using UnityEngine;

namespace ProjectMER.Features.Components;

[BlockComponent(ComponentType.ScpDoor)]
public sealed class ScpDoor : MonoBehaviour, IBlockComponent, IInteractableObject
{
    public static readonly List<ScpDoor> AllDoors = [];
    public Door? Door { get; private set; }
    public RoleTypeId Role;
    public bool DoorBroken { get; private set; }
    public bool DoorOpenedBefore { get; set; }
    
    public ComponentData Compile()
    {
        return new ComponentData()
        {
            Type = ComponentType.ScpDoor,
            Properties = new Dictionary<string, object>()
            {
                { nameof(Role), Role }
            }
        };
    }

    public void Decompile(ComponentData componentData)
    {
        if (componentData.Properties.TryGetValue("Role", out var roleObj))
        {
            Role = roleObj is JToken token
                ? token.ToObject<RoleTypeId>()
                : (RoleTypeId)Convert.ToSByte(roleObj);
        }
    }

    public void Awake()
    {
        AllDoors.Add(this);
        var door = GetComponent<DoorVariant>();
        if (door == null)
            return;
        Door = Door.Get(door);
    }

    public void OnDestroy()
    {
        AllDoors.Remove(this);
    }

    public void BreakDoor()
    {
        if (Door == null)
            return;
        
        Door.IsOpened = true;
        Door.Lock(DoorLockReason.SpecialDoorFeature, true);
        DoorBroken = true;
        DoorOpenedBefore = true;
    }

    public void RepairDoor()
    {
        DoorBroken = false;
        Door?.Lock(DoorLockReason.SpecialDoorFeature, false);
    }

    public bool CanInteract(Player player)
    {
        if (player.IsDestroyed || !DoorBroken)
            return false;
        if (player.CurrentItem == null || !player.CurrentItem.TryGetCustomItem(out var item))
            return false;
        return true;
    }

    public void Interact(Player player)
    {
        throw new NotImplementedException();
    }
}
