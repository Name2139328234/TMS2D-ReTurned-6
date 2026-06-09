using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnTilePreference : MonoBehaviour
{
    public HashSet<TileBase>Tiles { get => _tiles.ToHashSet(); }

    [SerializeField] private TileBase[] _tiles;
}
