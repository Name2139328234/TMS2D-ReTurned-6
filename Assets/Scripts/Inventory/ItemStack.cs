using System;



[Serializable]
public struct ItemStack
{
    public int Count;
    public ItemKind Kind;



    public ItemStack(int count, ItemKind kind)
    {
        Count = count;
        Kind = kind;
    }
}
