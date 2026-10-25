using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [Header("Box prefab to spawn")]
    public GameObject smallBoxPrefab;
    public GameObject bigBoxPrefab;
    public InventoryUI ui;
    [SerializeField]
    private Dictionary<BoxType, int> inventory = new Dictionary<BoxType, int>()
    {
        {BoxType.Small,0},
        {BoxType.Big,0},
    };
    private PlayerMovement2D movement;
    private void Awake() => movement = GetComponent<PlayerMovement2D>();
    private void Start() => UpdateUI();

    private void OnEnable() => MobileGameInput.ActionPressed += OnTouchAction;
    private void OnDisable() => MobileGameInput.ActionPressed -= OnTouchAction;

    private void OnTouchAction(MobileGameInput.Control control)
    {
        if (control == MobileGameInput.Control.SmallBox) SpawnSmallBox();
        if (control == MobileGameInput.Control.BigBox) SpawnBigBox();
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.TryGetComponent(out CollectibleBox box))
        {
            if (!box.canPickUp) return;     //don't pick up world box or after it's spawned
            box.canPickUp = false;
            inventory[box.boxType]++;   //increaes count
            Destroy(other.gameObject);
            Debug.Log("Pick Up : " + box.boxType);
            UpdateUI();
        }
    }

    private void Update()
    {
        if (Time.timeScale <= 0f || Keyboard.current == null || (movement && !movement.CanAcceptInput)) return;
        if (Keyboard.current.digit1Key.wasPressedThisFrame) //press 1 for small box
        {
            SpawnBox(BoxType.Small);
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame) //press 2 for big box
        {
            SpawnBox(BoxType.Big);
        }
    }

    public void SpawnSmallBox()
    {
        if (isActiveAndEnabled && Time.timeScale > 0f) SpawnBox(BoxType.Small);
    }

    public void SpawnBigBox()
    {
        if (isActiveAndEnabled && Time.timeScale > 0f) SpawnBox(BoxType.Big);
    }

    private void SpawnBox(BoxType type)
    {
        if (movement && !movement.CanAcceptInput) return;
        if (inventory[type] <= 0)
        {
            Debug.Log("No" + type + "in inventory");
            return;
        }


        //spawn at player position with an offset
        Vector3 spawnPos = transform.position + Vector3.right; //spawn slight to the right
        GameObject prefab = (type == BoxType.Small) ? smallBoxPrefab : bigBoxPrefab;
        if (!prefab) return;
        GameObject newBox = Instantiate(prefab, spawnPos, Quaternion.identity);
        inventory[type]--; // consume only after a valid spawn

        if (newBox.TryGetComponent(out CollectibleBox box))
        {
            box.canPickUp = false;
        }
        Debug.Log("Spawn : " + type);
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (ui != null)
        {
            ui.UpdateInventory(inventory); 
        }
    }
}
