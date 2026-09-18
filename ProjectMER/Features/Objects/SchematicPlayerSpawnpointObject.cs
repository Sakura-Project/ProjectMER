using PlayerRoles;
using Sakura.CustomRoles;
using UnityEngine;

namespace ProjectMER.Features.Objects;

public class SchematicPlayerSpawnpointObject : MonoBehaviour
{
    public static readonly List<SchematicPlayerSpawnpointObject> SpawnpointObjects = new();
    public List<RoleTypeId> Roles { get; set; } = [];
    public List<CustomRoleType> CustomRoles { get; set; } = [];
    
    public void OnEnable()
    {
        SpawnpointObjects.Add(this);
    }

    public void OnDisable()
    {
        SpawnpointObjects.Remove(this);
    }
}