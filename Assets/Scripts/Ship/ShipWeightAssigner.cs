using ObservableCollections;
using System;
using System.Collections.Generic;
using UnityEngine;



public class ShipWeightAssigner : MonoBehaviour
{
    [SerializeField] private Ship _ship;
    [SerializeField] private Rigidbody2D _shipBody;

    private float _totalWeight;



    void Awake()
    {
        _ship.Parts.CollectionChanged += OnShipChanged;
    }

    private void OnShipChanged(in NotifyCollectionChangedEventArgs<KeyValuePair<Vector2Int, GameObject>> e)
    {
        if (!e.IsSingleItem)
            throw new Exception("Can't handle more than one ship part change at a time yet");

        var weight = e.NewItem.Value.GetComponent<PartWeight>();

        switch (e.Action)
        {
            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                if (weight != null)
                    ReWeight(weight.Value);
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                if (weight != null)
                    ReWeight(-weight.Value);
                break;
            default:
                throw new Exception($"Unexpected action {e.Action} can't be handled yet");
        }

    }

    private void ReWeight(float delta)
    {
        _totalWeight += delta;
        _shipBody.mass = _totalWeight;
    }
}
