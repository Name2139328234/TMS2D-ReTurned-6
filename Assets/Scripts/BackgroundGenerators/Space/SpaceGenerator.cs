using UnityEngine;
using UnityEngine.Tilemaps;

public class SpaceGenerator : MonoBehaviour
{
    [SerializeField] private BoundsInt _bounds;
    [SerializeField] private Tilemap _space;
    [SerializeField] private Tile[] _tiles;



    private void Start()
    {
        for (int x = _bounds.xMin; x < _bounds.xMax; x++)
            for (int y = _bounds.yMin; y < _bounds.yMax; y++)
                _space.SetTile(new Vector3Int(x, y), _tiles[Random.Range(0, _tiles.Length)]);
    }
}
