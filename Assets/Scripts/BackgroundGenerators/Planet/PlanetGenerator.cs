//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;



public class PlanetGenerator : MonoBehaviour
{
    public Vector2Int ChunkSize { get => _chunkSize; }
    public int GenerationRange { get => _generationRange; }
    public Tilemap Ground { get => _ground; }
    public PlanetTilesHolder Tiles { get => _tiles; }

    [SerializeField] private Transform _player;
    [SerializeField] private Vector2Int _chunkSize;
    [SerializeField] private int _generationRange;
    [SerializeField] private Tilemap _ground;
    [SerializeField] private PlanetTilesHolder _tiles;
    [SerializeField] private int _heightSeed;
    [SerializeField] private int _temperatureSeed;
    [SerializeField] private float _globalHeightOffset;
    [SerializeField] private float _globalTemperatureOffset;
    [SerializeField] private float _globalHeightMultiplier;
    [SerializeField] private float _globalTemperatureMultiplier;
    [SerializeField] private float _heightNoiseFrequency;
    [SerializeField] private float _temperatureNoiseFrequency;
    [SerializeField] private int _heightNoiseOctaves;
    [SerializeField] private int _temparatureNoiseOctaves;

    //private HashSet<Vector2Int> _generatedChunks = new();
    private FastNoise _heightNoise;
    private FastNoise _temperatureNoise;

    

    void Awake()
    {
        _heightNoise = new(_temperatureSeed);
        _temperatureNoise = new(_heightSeed);

        _heightNoise.SetNoiseType(FastNoise.NoiseType.PerlinFractal);
        _temperatureNoise.SetNoiseType(FastNoise.NoiseType.PerlinFractal);

        _heightNoise.SetFrequency(_heightNoiseFrequency);
        _temperatureNoise.SetFrequency(_temperatureNoiseFrequency);

        _heightNoise.SetFractalOctaves(_heightNoiseOctaves);
        _temperatureNoise.SetFractalOctaves(_temparatureNoiseOctaves);


        var playerChunk = GetPlayerChunk();

        for (int x = -_generationRange; x <= _generationRange; x++)
        {
            for (int y = -_generationRange; y <= _generationRange; y++)
            {
                Vector2Int offset = new(x, y);

                GenerateChunk(playerChunk + offset);
            }
        }
    }
    /*
    void Update()
    {
        var playerChunk = GetPlayerChunk();

        for (int x = -_generationRange; x <= _generationRange; x++)
        {
            for (int y = -_generationRange; y <= _generationRange; y++)
            {
                Vector2Int offset = new(x, y);

                if (_generatedChunks.Contains(playerChunk + offset))
                    continue;

                GenerateChunk(playerChunk + offset);
            }
        }
    }
    */



#if UNITY_EDITOR
    [ContextMenu("RandomizeSeeds")]
    public void RandomizeSeeds()
    {
        _heightSeed = Random.Range(0, 1000);
        _temperatureSeed = Random.Range(0, 1000);
    }
#endif



    private Vector2Int GetPlayerChunk()
    {
        var scaledPos = _ground.transform.worldToLocalMatrix.MultiplyPoint(_player.position);

        return Vector2Int.FloorToInt(new Vector2(scaledPos.x / _chunkSize.x, scaledPos.y / _chunkSize.y));
    }
    private void GenerateChunk(Vector2Int index)
    {
        Vector2Int offset = new(index.x * _chunkSize.x, index.y * _chunkSize.y);

        for (int x = 0; x < _chunkSize.x; x++)
        {
            for (int y = 0; y < _chunkSize.y; y++)
            {
                var globalPos = new Vector2Int(x, y) + offset;

                _ground.SetTile((Vector3Int)globalPos, GetBestTile(globalPos));
            }
        }

        //_generatedChunks.Add(index);
    }
    private Tile GetBestTile(Vector2Int globalPos)
    {
        Tile result = null;
        float delta = float.PositiveInfinity;

        var tiles = _tiles.TilesGrid;
        float heightNoiseValue = _heightNoise.GetNoise(globalPos.x, globalPos.y);
        float temperatureNoiseValue = _temperatureNoise.GetNoise(globalPos.x, globalPos.y);

        for (int width = 0; width < _tiles.GridWidth; width++)
        {
            for (int height = 0; height < _tiles.GridHeight; height++)
            {
                float deltaHeight = Mathf.Abs(heightNoiseValue * _globalHeightMultiplier + _globalHeightOffset - width / (float)_tiles.GridWidth);
                float deltaTemperature = Mathf.Abs(temperatureNoiseValue * _globalTemperatureMultiplier + _globalTemperatureOffset - height / (float)_tiles.GridHeight);

                if (deltaHeight + deltaTemperature < delta)
                {
                    result = tiles[width, height];
                    delta = deltaHeight + deltaTemperature;
                }
            }
        }

        return result;
    }
}
