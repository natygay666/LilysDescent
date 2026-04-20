using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Transform gridParent; 
    public GameObject itemSlotPrefab;

    public void AddItem(UpgradeItem item)
    {
        GameObject slot = Instantiate(itemSlotPrefab, gridParent);

        InventorySlotUI slotUI = slot.GetComponent<InventorySlotUI>();
        slotUI.Setup(item);
    }
}