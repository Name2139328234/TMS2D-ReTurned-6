using Cysharp.Threading.Tasks;
using GalaxyUtilities;
using LitMotion;
using LitMotion.Extensions;
using R3;
using ReactiveInputSystem;
using Reflex.Attributes;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;



public class GalaxyPointSelector : MonoBehaviour//TODO split this class
{
    [SerializeField] private CameraScaler _scaler;
    [SerializeField] private float _completionTime;
    [SerializeField] private float _pressIntervalSeconds;
    [SerializeField] private float _cellSize;
    [SerializeField] private Tile _blank;
    [SerializeField] private Tilemap _enemyMap;
    [SerializeField] private Tilemap _traderMap;
    [SerializeField] private Tilemap _spawnableMap;
    [SerializeField] private int _galaxyRadius;
    [SerializeField][Range(0f, 1f)] private float _planetChance;
    [SerializeField] private int _planetSeed;
    [SerializeField] private int _spawnablesSeed;
    [SerializeField] private int _enemySeed;
    [SerializeField] private int _traderSeed;
    [SerializeField] private float _enemyMultiplier;
    [SerializeField] private float _traderMultiplier;
    [SerializeField] private float _enemyFloor;
    [SerializeField] private float _traderFloor;
    [SerializeField] private string _planetSceneName;
    [SerializeField] private string _spaceSceneName;

    [Inject] private PlayerInputActions _input;

    private Dictionary<Vector2Int, QuadrantInfo> _quadrants;
    private DisposableBag _disposables;
    private CancellationTokenSource _cts;



    void Start()
    {
        GenerateGalaxy();

        _cts = new CancellationTokenSource().AddTo(ref _disposables);

        _input.Player.SpecialSelect.Enable();
        _input.AddTo(ref _disposables);

        //this exists since it's impossible to pass a Vector2 via Input on double click
        _input.Player.SpecialSelect
            .PerformedAsObservable(_cts.Token)
            .Select(context => GetTileMiddle())
            .Subscribe(desiredPos => Animate(desiredPos).Forget())
            .AddTo(ref _disposables);
    }
    void OnDestroy()
    {
        _cts.Cancel();
        _input.Player.SpecialSelect.Disable();
        _disposables.Dispose();
    }



    private void GenerateGalaxy()
    {
        _quadrants = new();

        System.Random random = new(_planetSeed);
        FastNoise enemyNoise = new(_enemySeed);
        FastNoise traderNoise = new(_traderSeed);
        enemyNoise.SetNoiseType(FastNoise.NoiseType.PerlinFractal);
        traderNoise.SetNoiseType(FastNoise.NoiseType.PerlinFractal);
        enemyNoise.SetFractalOctaves(4);
        traderNoise.SetFractalOctaves(4);
        enemyNoise.SetFrequency(0.04f);
        traderNoise.SetFrequency(0.04f);


        for (int x = -_galaxyRadius; x < _galaxyRadius; x++)
        {
            for (int y = -_galaxyRadius; y < _galaxyRadius; y++)
            {
                if (x * x + y * y < _galaxyRadius * _galaxyRadius)//circle check
                {
                    QuadrantKind kind = random.Next(0, 100) / 100f < _planetChance ? QuadrantKind.Planet : QuadrantKind.Space;
                    int enemies = (int)(enemyNoise.GetNoise(x, y) * _enemyMultiplier + _enemyFloor);
                    int traders = (int)(traderNoise.GetNoise(x, y) * _traderMultiplier + _traderFloor);

                    QuadrantInfo info = new(kind, enemies, traders);
                    _quadrants.Add(new Vector2Int(x, y), info);
                }
            }
        }

        foreach (var quadrant in _quadrants)
        {
            _enemyMap.SetTile((Vector3Int)quadrant.Key, _blank);
            _enemyMap.SetColor((Vector3Int)quadrant.Key, Color.Lerp(Color.white, Color.red, quadrant.Value.EnemyWaveCount / 10f));

            _traderMap.SetTile((Vector3Int)quadrant.Key, _blank);
            _traderMap.SetColor((Vector3Int)quadrant.Key, Color.Lerp(Color.white, Color.green, quadrant.Value.TradeStationCount / 10f));

            _spawnableMap.SetTile((Vector3Int)quadrant.Key, _blank);
            _spawnableMap.SetColor((Vector3Int)quadrant.Key, Color.Lerp(Color.white, Color.blue, quadrant.Value.GetAverageSpawnProbability()));
        }
    }
    private async UniTask Animate(Vector3 desiredPos)
    {
        AnimatePos(desiredPos);
        AnimateSize();

        await UniTask.Delay(TimeSpan.FromSeconds(_completionTime), cancellationToken: _cts.Token);

        Vector2Int quadrantPos = Vector2Int.FloorToInt(desiredPos /= _cellSize);

        if (!_quadrants.ContainsKey(quadrantPos))
            return;

        Serializer.SerializeQuadrant(_quadrants[quadrantPos]);
        SceneManager.LoadScene(_quadrants[quadrantPos].Kind == QuadrantKind.Space ? _spaceSceneName : _planetSceneName);
    }
    private void AnimatePos(Vector3 desiredPos)
    {
        desiredPos.z = transform.position.z;

        LMotion.Create(transform.position, desiredPos, _completionTime)
            .WithEase(Ease.InOutSine)
            .BindToPosition(transform)
            .AddTo(ref _disposables);
    }
    private void AnimateSize()
    {
        LMotion.Create(_scaler.Camera.orthographicSize, _scaler.Min, _completionTime)
            .WithEase(Ease.InOutCubic)
            .BindToOrthographicSize(_scaler.Camera)
            .AddTo(ref _disposables);
    }
    private Vector3 GetTileMiddle()
    {
        Vector3 worldPos = _scaler.Camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        var result = worldPos;

        result /= _cellSize;
        result = Vector3Int.FloorToInt(result);
        result += Vector3.one * 0.5f;
        result *= _cellSize;

        return result;
    }
}



namespace GalaxyUtilities
{
    public struct QuadrantInfo
    {
        public IReadOnlyDictionary<SpawnableKind, float> SpawnProbability { get => _spawnProbability; }

        public QuadrantKind Kind;
        public int EnemyWaveCount;
        public int TradeStationCount;

        private Dictionary<SpawnableKind, float> _spawnProbability;



        public QuadrantInfo(QuadrantKind kind, int enemyCount, int tradersCount)
        {
            Kind = kind;
            EnemyWaveCount = enemyCount;
            TradeStationCount = tradersCount;
            _spawnProbability = new();

            System.Random random = new();//TODO pass a seed

            foreach (SpawnableKind spawnable in QuadrantKindSpawnables.Get(kind))
            {
                _spawnProbability.Add(spawnable, random.Next(0, 10) / 10f);
            }
        }
        public QuadrantInfo(QuadrantKind kind, int enemyCount, int tradersCount, Dictionary<SpawnableKind, float> probabilities)
        {
            Kind = kind;
            EnemyWaveCount = enemyCount;
            TradeStationCount = tradersCount;

            _spawnProbability = probabilities;
        }



        public float GetAverageSpawnProbability()
        {
            //return _spawnProbability[SpawnableKind.AsteroidIron];

            float result = 0f;

            foreach (var chance in _spawnProbability.Values)
            {
                result += chance;
            }

            result /= _spawnProbability.Count;
            return result;
        }
    }
    public enum QuadrantKind
    {
        Space,
        Planet
    }
    public enum SpawnableKind
    {
        AsteroidIron,
        AsteroidLead,
        AsteroidUranium,
        AsteroidCopper,
        AsteroidSilver,
        AsteroidGold,
        SiteMine,
        SiteFarm,
        SiteFactory
    }
    public static class QuadrantKindSpawnables
    {
        private static Dictionary<QuadrantKind, SpawnableKind[]> _pairs = new()
        {
            {
                QuadrantKind.Space,
                new SpawnableKind[]
                {
                    SpawnableKind.AsteroidIron,
                    SpawnableKind.AsteroidLead,
                    SpawnableKind.AsteroidUranium,
                    SpawnableKind.AsteroidCopper,
                    SpawnableKind.AsteroidSilver,
                    SpawnableKind.AsteroidGold
                }
            },
            {
                QuadrantKind.Planet,
                new SpawnableKind[]
                {
                    SpawnableKind.SiteMine,
                    SpawnableKind.SiteFarm,
                    SpawnableKind.SiteFactory
                }
            }
        };



        public static IEnumerable<SpawnableKind> Get(QuadrantKind quadrantKind)
        {
            return _pairs[quadrantKind];
        }
    }
}