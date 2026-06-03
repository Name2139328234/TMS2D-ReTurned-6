using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class Builder : MonoBehaviour
{
    public UnityAction<BuildingArgs> OnBuild;
    public UnityAction<BuildingArgs> OnUnbuild;

    [HideInInspector][NonSerialized] public Vector2Int SelectedPosition;
    [HideInInspector][NonSerialized] public PartInfo SelectedInfo;

    public PartPriceInfo[] AvailableParts { get => _builderBlocks.AvailableParts; }
    public Ship Ship { get => _ship; }
    public Inventory Inventory { get => _inventory; }

    [SerializeField] private BuilderBlocks _builderBlocks;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private Ship _ship;

    private bool _isAllowedBuilding = true;



    public void Build(bool isSpending)
    {
        if (!_isAllowedBuilding)
            return;

        var info = GetPriceInfo(SelectedInfo.Kind);
        var cost = info.GetLevelledCost(SelectedInfo.Level);


        if (isSpending)
        {
            if (!_inventory.IsEnough(cost))
                return;

            foreach (var stack in cost)
            {
                _inventory.Remove(stack);
            }
        }

        var spawned = Instantiate(info.PartLevels[SelectedInfo.Level], _ship.transform);
        spawned.name = info.PartLevels[SelectedInfo.Level].name;
        spawned.transform.SetLocalPositionAndRotation((Vector3Int)SelectedPosition, Quaternion.identity);
        spawned.layer = _ship.gameObject.layer;
        _ship.AddPart(SelectedPosition, spawned);

        OnBuild?.Invoke(new(isSpending));
    }
    public void Unbuild(bool isGaining)
    {
        if (!_isAllowedBuilding)
            return;

        var partGO = _ship.RemovePart(SelectedPosition);

        if (partGO == null)
            return;

        if (isGaining)
        {
            foreach (var partInfo in _builderBlocks.AvailableParts)
            {
                for (int i = 0; i < partInfo.PartLevels.Length; i++)
                {
                    if (partGO.name == partInfo.PartLevels[i].name)
                    {
                        var cost = partInfo.GetLevelledCost(i);

                        foreach (var stack in cost)
                        {
                            _inventory.Add(stack);
                        }
                    }
                }
            }
        }

        Destroy(partGO);

        OnUnbuild?.Invoke(new(isGaining));
    }
    public PartPriceInfo GetPriceInfo(PartKind kind)
    {
        foreach (var partInfo in _builderBlocks.AvailableParts)
            if (partInfo.Kind == kind)
                return partInfo;

        throw new Exception($"No part of PartKind {kind} found");
    }
    public bool IsEnough()
    {
        return _inventory.IsEnough(GetPriceInfo(SelectedInfo.Kind).GetLevelledCost(SelectedInfo.Level));
    }
    public void SetAllowedBuilding(bool isAllowed)
    {
        _isAllowedBuilding = isAllowed;
    }
}



[Serializable]
public class PartPriceInfo
{
    public PartKind Kind;
    public GameObject[] PartLevels;
    public ItemStack[] CostBase;



    public ItemStack[] GetLevelledCost(int level)
    {
        List<ItemStack> cost = new();

        foreach (var stack in CostBase)
        {
            cost.Add(new(stack.Kind, (int)Mathf.Pow(4, level) * stack.Count));
        }

        return cost.ToArray();
    }
}
public struct BuildingArgs
{
    public bool IsPriced;



    public BuildingArgs(bool isPriced)
    {
        IsPriced = isPriced;
    }
}