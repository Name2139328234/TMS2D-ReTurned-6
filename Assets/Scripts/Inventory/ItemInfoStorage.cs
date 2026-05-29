using System.Collections.Generic;
using UnityEngine;


//TODO inject this with DI, not manually in editor
[CreateAssetMenu(fileName = "ItemInfoStorage", menuName = "Custom/ItemInfoStorage", order = 1)] 
public class ItemInfoStorage : ScriptableObject
{
    public IReadOnlyList<ItemInfo> ItemInfos { get => itemInfos; }

    [SerializeField] private ItemInfo[] itemInfos;



    public ItemInfo Get(ItemKind kind)
    {
        foreach (var info in itemInfos)
        {
            if (info.Kind == kind)
                return info;
        }

        throw new System.Exception($"no info for {kind} ItemKind found");
    }
}