using UnityEngine;



public class Item : MonoBehaviour
{
    public ItemKind Kind { get => _kind; }

    [SerializeField] private ItemKind _kind;
}
