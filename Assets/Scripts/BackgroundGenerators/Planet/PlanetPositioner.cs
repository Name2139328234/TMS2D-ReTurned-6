using Closures;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using ZLinq;



public class PlanetPositioner : IPositioner
{
    private PlanetGenerator _generator;
    private HashSet<Vector2Int> _takenPositions = new();



    public PlanetPositioner(PlanetGenerator generator)
    {
        _generator = generator;
    }
    public Vector3 GetPosition(GameObject target)
    {
        var tilePref = target.GetComponent<SpawnTilePreference>();

        if (tilePref == null || !Contains(tilePref.Tiles))
            throw new System.Exception("No prefrence for a valid tile was found");

        Dictionary<Vector2Int, TileBase> tiles = new();

        BoundsInt bounds = _generator.Ground.cellBounds;

        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                Vector2Int pos = new(x, y);
                var tile = _generator.Ground.GetTile((Vector3Int)pos);

                if (tile != null)
                    tiles.Add(pos, tile);

            }
        }

        var bestTile = tiles
            .AsValueEnumerable()
            .Where(Closure.Func<SpawnTilePreference, KeyValuePair<Vector2Int, TileBase>, bool>(tilePref, (pref, tilePos) => pref.Tiles.Contains(tilePos.Value)).AsFunc())//consider using a struct with both pieces of data here
            .Where(Closure.Func<HashSet<Vector2Int>, KeyValuePair<Vector2Int, TileBase>, bool>(_takenPositions, (positions, tilePos) => !positions.Contains(tilePos.Key)).AsFunc())
            .MinBy(Closure.Func<PlanetGenerator, KeyValuePair<Vector2Int, TileBase>, float>(_generator, (generator, tilePos) => Vector3.Distance((generator.ChunkSize.x + generator.ChunkSize.y) * 0.5f * generator.GenerationRange * Random.insideUnitCircle, (Vector3Int)tilePos.Key)).AsFunc());

        _takenPositions.Add(bestTile.Key);

        return _generator.Ground.transform.localToWorldMatrix.MultiplyPoint((Vector3Int)bestTile.Key);
    }
    public Quaternion GetRotation()
    {
        return Quaternion.identity;
    }



    private bool Contains(HashSet<TileBase> tiles)
    {
        foreach (var tile in tiles)
        {
            if (_generator.Tiles.GeneratedTilesUnordered.Contains((Tile)tile))
                return true;
        }

        return false;
    }
}
