using ObservableCollections;
using System;
using System.Collections.Generic;
using UnityEngine;



public class EnergyNetwork : MonoBehaviour
{
    [SerializeField] private Ship _ship;

    private Dictionary<Vector2Int, EnergyGenerator> _powerGenerators = new();
    private Dictionary<Vector2Int, EnergyUser> _powerUsers = new();
    private Dictionary<Vector2Int, Wire> _wires = new();
    private List<NetworkPart> _subNetworks = new();



    void Start()
    {
        _ship.Parts.CollectionChanged += OnShipChanged;
    }
    void Update()
    {
        foreach (var subNetwork in _subNetworks)
        {
            float totalProduction = 0f;
            foreach (var generator in subNetwork.Generators)
            {
                totalProduction += generator.Production;
            }

            float totalDemand = 0f;
            foreach (var user in subNetwork.Users)
            {
                totalDemand += user.UseAmount;
            }

            float producedEnergy = totalProduction * Time.deltaTime;
            foreach (var user in subNetwork.Users)
            {
                float share = producedEnergy * (user.UseAmount / totalDemand);
                user.GiveEnergy(share);
            }
        }
    }



    private void OnShipChanged(in NotifyCollectionChangedEventArgs<KeyValuePair<Vector2Int, GameObject>> e)
    {
        if (!e.IsSingleItem)
            throw new Exception("Can't handle more than one ship part change at a time yet");

        var pos = Vector2Int.FloorToInt(e.NewItem.Value.transform.localPosition);
        var wire = e.NewItem.Value.GetComponent<Wire>();
        var generator = e.NewItem.Value.GetComponent<EnergyGenerator>();
        var user = e.NewItem.Value.GetComponent<EnergyUser>();

        switch (e.Action)
        {
            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                if (wire != null)
                    _wires.Add(pos, wire);
                if (generator != null)
                    _powerGenerators.Add(pos, generator);
                if (user != null)
                    _powerUsers.Add(pos, user);
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                if (wire != null)
                    _wires.Remove(pos);
                if (generator != null)
                    _powerGenerators.Remove(pos);
                if (user != null)
                    _powerUsers.Remove(pos);
                break;
            default:
                throw new Exception($"Unexpected action {e.Action} can't be handled yet");
        }

        if (wire != null)//all network parts are also marked as wires, so their checks are not required
            UpdateNetworkParts();
    }
    private void UpdateNetworkParts()
    {
        _subNetworks.Clear();

        if (_wires.Count == 0) return;

        HashSet<Vector2Int> visited = new();

        foreach (var startPos in _wires.Keys)
        {
            if (visited.Contains(startPos)) continue;

            HashSet<Vector2Int> networkPart = new();
            Queue<Vector2Int> queue = new();
            queue.Enqueue(startPos);
            visited.Add(startPos);
            networkPart.Add(startPos);

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                foreach (Vector2Int dir in new[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
                {
                    Vector2Int neighbor = current + dir;
                    if (_wires.ContainsKey(neighbor) && !visited.Contains(neighbor))
                    {
                        visited.Add(neighbor);
                        queue.Enqueue(neighbor);
                        networkPart.Add(neighbor);
                    }
                }
            }

            List<EnergyGenerator> generatorsInNetworkPart = new();
            foreach (var generatorPos in _powerGenerators)
            {
                if (networkPart.Contains(generatorPos.Key))
                {
                    generatorsInNetworkPart.Add(generatorPos.Value);
                }
            }

            List<EnergyUser> usersInNetworkPart = new();
            foreach (var userPos in _powerUsers)
            {
                if (networkPart.Contains(userPos.Key))
                {
                    usersInNetworkPart.Add(userPos.Value);
                }
            }

            if (usersInNetworkPart.Count == 0 || generatorsInNetworkPart.Count == 0)
                continue;

            _subNetworks.Add(new NetworkPart(networkPart, generatorsInNetworkPart, usersInNetworkPart));
        }
    }



    private struct NetworkPart
    {
        public HashSet<Vector2Int> Positions;
        public List<EnergyGenerator> Generators;
        public List<EnergyUser> Users;



        public NetworkPart(IEnumerable<Vector2Int> positions, IEnumerable<EnergyGenerator> generators, IEnumerable<EnergyUser> users)
        {
            Positions = new(positions);
            Generators = new(generators);
            Users = new(users);
        }
    }
}
