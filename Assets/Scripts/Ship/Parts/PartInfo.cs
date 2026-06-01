using System;
using UnityEngine;



[Serializable]
public struct PartInfo
{
    public PartKind Kind { get => _kind; }
    public int Level { get => _level; }

    [SerializeField] private PartKind _kind;
    [SerializeField] private int _level;



    public PartInfo(PartKind kind, int level)
    {
        _kind = kind;
        _level = level;
    }
}
