using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class Inventory : MonoBehaviour
{
    public UnityAction OnChange;

    public IReadOnlyList<ItemStack> ItemStacks { get => _itemStacks; }

    [SerializeField] private List<ItemStack> _itemStacks;



    void Start()
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
        int combinedCount = 0;
        foreach (var stack in _itemStacks)
            if (stack.Kind == comparedStack.Kind)
                combinedCount += stack.Count;

        return combinedCount >= comparedStack.Count;
    }
    public void Add(ItemStack otherStack)
    {
        for (int i = 0; i < _itemStacks.Count; i++)//not foreach because modifications of stack do not carry over
        {
            if (_itemStacks[i].Kind == otherStack.Kind)
            {
                _itemStacks[i] = new (_itemStacks[i].Count + otherStack.Count, _itemStacks[i].Kind);//this cannot be modified directly either
                OnChange?.Invoke();
                return;
            }
        }
        _itemStacks.Add(otherStack);
        OnChange?.Invoke();

    }
    public void Remove(ItemStack otherStack)
    {
        for (int i = 0; i < _itemStacks.Count; i++)//same as add
        {
            ItemStack stack = _itemStacks[i];
            if (stack.Kind == otherStack.Kind && stack.Count >= otherStack.Count)
            {
                stack.Count -= otherStack.Count;//same as add
                _itemStacks[i] = stack;
                OnChange?.Invoke();
                return;
            }
        }

        Debug.LogError("attempted to remove item that isn't there or there is too few of it");
    }
    public void Clear()
    {
        _itemStacks.Clear();
        Initialize();
        OnChange?.Invoke();
    }



    private void Initialize()
    {
        foreach (ItemKind kind in Enum.GetValues(typeof(ItemKind)))
        {
            Add(new ItemStack(0, kind));
        }
    }
}
