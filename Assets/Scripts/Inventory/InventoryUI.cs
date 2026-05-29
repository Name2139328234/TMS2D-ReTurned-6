using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory _inventory;
    [SerializeField] private ItemInfoStorage _icons;
    [SerializeField] private GameObject _itemUIPrefab;
    [SerializeField] private Transform _UIParent;
    [SerializeField] private Vector2 _deltaPosition;
    
    private List<GameObject> _itemUIs = new();



    void Start()
    {
        Regenerate();
        _inventory.OnChange += Regenerate;
    }



    public void SetTargetedInventory(Inventory inventory)
    {
        if (_inventory != null)
            _inventory.OnChange -= Regenerate;

        _inventory = inventory;
        Regenerate();
        _inventory.OnChange += Regenerate;
    }



    private void Regenerate()
    {
        foreach (var oldItemUI in _itemUIs)
        {
            Destroy(oldItemUI);
        }

        _itemUIs = new();

        for (int i = 0; i < _inventory.ItemStacks.Count; i++)
        {
            var spawned = Instantiate(_itemUIPrefab, _UIParent);
            spawned.transform.localPosition = _deltaPosition * i;
            spawned.GetComponent<ItemUI>().Initiate(_icons.Get(_inventory.ItemStacks[i].Kind).Preview, _inventory.ItemStacks[i].Count);
            _itemUIs.Add(spawned);
        }
    }
}
