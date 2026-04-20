using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image icon;

    public void Setup(UpgradeItem item)
    {
        icon.sprite = item.icon;
    }
}