using InventorySystem.Items;
using LabApi.Features.Wrappers;
using Mirror;
using ProjectMER.Features;
using ProjectMER.Features.ToolGun;
using Sakura.API.ServerSpecific;

namespace ProjectMER.Settings;

internal sealed class SchematicName : CustomDropdownSetting
{
    public const string Text = "Schematic Name";
    public SchematicName() : base(
        Text.GetStableHashCode(),
        Text,
        MapUtils.GetAvailableSchematicNames()
        )
    {
    }

    public override CustomHeader Header => SettingsRegistry.ProjectMerHeader;

    protected override CustomSetting CreateDuplicate() => new SchematicName();

    protected override bool CanView(Player player)
    {
        foreach (ItemBase itemBase in player.Inventory.UserInventory.Items.Values)
        {
            if (ToolGunItem.ItemDictionary.ContainsKey(itemBase.ItemSerial))
            {
                return true;
            }
        }

        return false;
    }

    protected override void HandleSettingUpdate()
    {
        
    }
}