using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<UpgradeItem> collectedItems = new List<UpgradeItem>();
    public InventoryUI inventoryUI;

    private int upgradeLayer;

    void Start()
    {
        upgradeLayer = LayerMask.NameToLayer("Upgrade");
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == upgradeLayer)
        {
            UpgradeItem item = other.GetComponent<UpgradeItem>();

            if (item != null)
            {
                collectedItems.Add(item);

                inventoryUI.AddItem(item);

                Destroy(other.gameObject);
            }
        }
    }
}