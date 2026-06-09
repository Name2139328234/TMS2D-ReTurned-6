using ObservableCollections;
using System;
using UnityEngine;



public class Ship : MonoBehaviour
{
    public Action<Ship> OnDead;

    public ObservableDictionary<Vector2Int, GameObject> Parts { get => _parts; }

    private ObservableDictionary<Vector2Int, GameObject> _parts = new();



    public bool IsOccupied(Vector2Int position)
    {
        return _parts.ContainsKey(position);
    }
    public void AddPart(Vector2Int position, GameObject part)
    {
        _parts.Add(position, part);
        //part.GetComponent<Health>().OnDie += RemoveDeadPart;
    }
    public GameObject RemovePart(Vector2Int position)
    {
        GameObject part = _parts[position];

        _parts.Remove(position);

        if (_parts.Count == 0)
            OnDead?.Invoke(this);

        return part;
    }
}
