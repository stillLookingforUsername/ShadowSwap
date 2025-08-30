using UnityEngine;

public enum BoxType { Small, Big }

public class CollectibleBox : MonoBehaviour
{
    public BoxType boxType;
    public bool canPickUp = true;
}