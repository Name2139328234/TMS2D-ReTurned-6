using GalaxyUtilities;
using Reflex.Attributes;
using System;
using System.Linq;
using UnityEngine;
using ZLinq;



public class Spawner : MonoBehaviour
{
    [SerializeField] private Spawnable[] _spawnables;

    [Inject] private IPositioner _positioner;

    private QuadrantInfo _quadrantInfo;
    


    void Start()
    {
        _quadrantInfo = Serializer.DeserializeQuadrant();

        var query = _spawnables
            .AsValueEnumerable()
            .Join(
            _quadrantInfo.SpawnProbability,
            spawnable => spawnable.Kind,
            probability => probability.Key,
            (spawnable, probability) => (spawnable.Prefab, probability.Value, spawnable.BaseCount)
            );

        foreach (var (Prefab, Value, BaseCount) in query)
            for (int i = 0; i < BaseCount * Value; i++)
                Spawn(Prefab);
    }



    private void Spawn(GameObject prefab)
    {
        var spawned = Instantiate(prefab);
        spawned.transform.SetPositionAndRotation(_positioner.GetPosition(spawned), _positioner.GetRotation());
    }



    [Serializable]
    private struct Spawnable
    {
        public SpawnableKind Kind;
        public GameObject Prefab;
        public int BaseCount;
    }
}