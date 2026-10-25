using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private string persistentId = "";
    private string collectionId;
    private bool collected;
    public string CollectionId
    {
        get
        {
            if (collectionId != null) return collectionId;
            string path = "";
            for (Transform node = transform; node; node = node.parent)
                path = node.name + "[" + node.GetSiblingIndex() + "]/" + path;
            collectionId = gameObject.scene.path + ":" + (string.IsNullOrEmpty(persistentId) ? path : persistentId);
            return collectionId;
        }
    }

    private void Awake() { _ = CollectionId; }
    public bool TryCollect()
    {
        if (collected) return false;
        collected = true;
        return true;
    }
    public void DestroySelf()
    {
        foreach (Collider2D hitbox in GetComponents<Collider2D>()) hitbox.enabled = false;
        Destroy(gameObject);
    }

}
