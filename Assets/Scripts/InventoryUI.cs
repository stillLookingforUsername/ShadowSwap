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
        Debug.Log("UpdateInventory is called");
        smallBox.text = "Small: " + inventory[BoxType.Small];
        bigBox.text = "Big: " + inventory[BoxType.Big];
    }
}