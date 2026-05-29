using UnityEngine;



[CreateAssetMenu(fileName = "ItemInfo", menuName = "Custom/ItemInfo", order = 0)]
public class ItemInfo : ScriptableObject
{
    public ItemKind Kind { get => _kind; }
    public GameObject Prefab { get => _prefab; }
    public Sprite Preview { get => _preview; }
    public int Price { get => _price; }

    [SerializeField] private ItemKind _kind;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Sprite _preview;
    [SerializeField] private int _price;
}
