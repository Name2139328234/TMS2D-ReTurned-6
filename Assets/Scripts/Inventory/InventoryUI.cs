using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;



public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] private ItemInfoStorage _icons;
    [SerializeField] private GameObject _itemUIPrefab;
    [SerializeField] private Transform _UIParent;
    [SerializeField] private Vector2 _deltaPosition;
    
    private Dictionary<ItemKind, ItemUI> _itemsUI = new();



    void Start()
    {
        Generate();
        _inventory.OnChange += UpdateUI;
    }



    private void UpdateUI(ItemStack stack)
    {
        _itemsUI[stack.Kind].UpdateCount(stack.Count);
    }
    private void Generate()
    {
        int index = 0;

        foreach (var kind in _inventory.ItemStacks.Keys)
        {
            var spawned = Instantiate(_itemUIPrefab, _UIParent);
            spawned.transform.localPosition = _deltaPosition * index;
            spawned.GetComponent<ItemUI>().Initiate(_icons.Get(kind).Preview, _inventory.ItemStacks[kind]);
            _itemsUI.Add(kind, spawned.GetComponent<ItemUI>());

            index++;
        }
    }
}
