using System;
using System.Collections.Generic;
using UnityEngine;



public class Builder : MonoBehaviour
{
    [HideInInspector] public Vector2Int SelectedPosition;
    [HideInInspector] public PartInfo SelectedInfo;

    public PartPriceInfo[] AvailableParts { get => _builderBlocks.AvailableParts; }

    [SerializeField] private BuilderBlocks _builderBlocks;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private Ship _ship;



    public void Build(bool isSpending)
    {
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
    }
    public void UnBuild()
    {
        var partGO = _ship.RemovePart(SelectedPosition);

        if (partGO == null)
            return;

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

        Destroy(partGO);
    }



    private PartPriceInfo GetPriceInfo(PartKind kind)
    {
        foreach (var partInfo in _builderBlocks.AvailableParts)
            if (partInfo.Kind == kind)
                return partInfo;

        throw new Exception($"No part of PartKind {kind} found");
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
            cost.Add(new((int)Mathf.Pow(4, level), stack.Kind));
        }

        return cost.ToArray();
    }
}