using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerInventory : MonoBehaviour
{
    [Header("Box prefab to spawn")]
    public GameObject smallBoxPrefab;
    public GameObject bigBoxPrefab;

    public List<BoxType> inventory = new List<BoxType>();

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out CollectibleBox box))
        {
            if (!box.canPickUp) return;     //don't pick up world box or after it's spawned
            inventory.Add(box.boxType);
            Destroy(other.gameObject);
            Debug.Log("Pick Up : " + box.boxType);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) //press 1 for small box
        {
            SpawnBox(BoxType.Small);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) //press 2 for big box
        {
            SpawnBox(BoxType.Big);
        }
    }

    private void SpawnBox(BoxType type)
    {
        if (!inventory.Contains(type))
        {
            Debug.Log("No" + type + "in inventory");
            return;
        }

        inventory.Remove(type); //remove first occurrence of  this box type

        //spawn at player position with an offset
        Vector3 spawnPos = transform.position + Vector3.right; //spawn slight to the right
        GameObject prefab = (type == BoxType.Small) ? smallBoxPrefab : bigBoxPrefab;
        GameObject newBox = Instantiate(prefab, spawnPos, Quaternion.identity);

        if (newBox.TryGetComponent(out CollectibleBox component))
        {
            component.canPickUp = false;
        }
        Debug.Log("Spawn : " + type);
    }
}