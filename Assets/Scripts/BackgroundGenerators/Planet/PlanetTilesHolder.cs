using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;



public class PlanetTilesHolder : MonoBehaviour
{
    public Tile[,] TilesGrid
    {
        get
        {
            Tile[,] result = new Tile[GridWidth, GridHeight];

            for (int y = 0; y < GridHeight; y++)
                for (int x = 0; x < GridWidth; x++)
                    result[x, y] = _generatedTiles[x + y * _gridWidth];

            return result;
        }
    }
    public List<Tile> GeneratedTilesUnordered { get => _generatedTiles; }
    public int GridWidth { get => _gridWidth; }
    public int GridHeight { get => _gridHeight; }

    [SerializeField] private List<Tile> _generatedTiles = new();
    [SerializeField][HideInInspector] private List<Sprite> _serializedTileSprites = new();
    [SerializeField] private int _gridWidth;
    [SerializeField] private int _gridHeight;
#if UNITY_EDITOR
    [SerializeField] private string _localTilePath;
#endif





#if UNITY_EDITOR
    void OnValidate()
    {
        //TODO make this process preserve 2D positions
        while (_serializedTileSprites.Count > _gridWidth * _gridHeight)
        {
            _serializedTileSprites.RemoveAt(0);
        }

        while (_serializedTileSprites.Count < _gridWidth * _gridHeight)
        {
            _serializedTileSprites.Add(null);
        }
    }




    [ContextMenu("GenerateTiles")]
    public void GenerateTiles()
    {
        foreach (var sprite in _serializedTileSprites)
        {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            AssetDatabase.CreateAsset(tile, Path.Combine(_localTilePath, sprite.name + ".asset"));
            _generatedTiles.Add(tile);
        }
    }
#endif
}
