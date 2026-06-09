using System;
using System.Collections.Generic;
using UnityEngine;



public class Inventory : MonoBehaviour
{
    public Action<ItemStack> OnChange;

    public IReadOnlyDictionary<ItemKind, int> ItemStacks { get => _itemStacks; }

    private Dictionary<ItemKind, int> _itemStacks = new();



    void Awake()
    {
        Initialize();
    }



    public bool IsEnough(IEnumerable<ItemStack> comparedStacks)
    {
        foreach (ItemStack stack in comparedStacks)
            if (!IsEnough(stack))
                return false;

        return true;
    }
    public bool IsEnough(ItemStack comparedStack)
    {
        return _itemStacks[comparedStack.Kind] >= comparedStack.Count;
    }
    public void Add(ItemStack otherStack)
    {
        _itemStacks[otherStack.Kind] += otherStack.Count;
        OnChange?.Invoke(new ItemStack(otherStack.Kind, _itemStacks[otherStack.Kind]));
    }
    public void Remove(ItemStack otherStack)
    {
        if (otherStack.Count > _itemStacks[otherStack.Kind])
            Debug.LogError("attempted to remove item that isn't there or there is too few of it");

        _itemStacks[otherStack.Kind] -= otherStack.Count;
        OnChange?.Invoke(new ItemStack(otherStack.Kind, _itemStacks[otherStack.Kind]));
    }



    private void Initialize()
    {
        foreach (ItemKind kind in Enum.GetValues(typeof(ItemKind)))
        {
            _itemStacks.Add(kind, 0);
        }
    }
}
