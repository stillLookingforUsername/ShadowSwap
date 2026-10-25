using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Image smallboxIcon;
    public TextMeshProUGUI smallBox;
    public Image bigBoxIcon;
    public TextMeshProUGUI bigBox;

    public void UpdateInventory(Dictionary<BoxType, int> inventory)
    {
        if (inventory == null) return;
        if (smallBox) smallBox.text = "Small: " + (inventory.TryGetValue(BoxType.Small, out int small) ? small : 0);
        if (bigBox) bigBox.text = "Big: " + (inventory.TryGetValue(BoxType.Big, out int big) ? big : 0);
    }
}
