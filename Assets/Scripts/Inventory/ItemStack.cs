using System;



[Serializable]
public struct ItemStack
{
    public ItemKind Kind;
    public int Count;



    public ItemStack(ItemKind kind, int count)
    {
        Kind = kind;
        Count = count;
    }
}
