using UnityEngine;
using UnityEngine.Tilemaps;



public class SpaceGenerator : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private BoundsInt _bounds;
    [SerializeField] private Tilemap _space;
    [SerializeField] private Tile[] _tiles;



    private void Update()
    {
        var playerPosScaled = Vector3Int.FloorToInt(_space.transform.worldToLocalMatrix.MultiplyPoint(_player.position));

        for (int x = _bounds.xMin; x < _bounds.xMax; x++)
            for (int y = _bounds.yMin; y < _bounds.yMax; y++)
                if (_space.GetTile(new Vector3Int(x, y) + playerPosScaled) == null)
                    _space.SetTile(new Vector3Int(x, y) + playerPosScaled, _tiles[Random.Range(0, _tiles.Length)]);
    }
}
