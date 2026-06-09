using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using ReactiveInputSystem;
using Reflex.Attributes;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;



public class Player : MonoBehaviour
{
    public Ship Ship { get => _ship; }
    public Builder Builder { get => _builder; }
    public Inventory Inventory { get => _inventory; }

    [SerializeField] private Ship _ship;
    [SerializeField] private Builder _builder;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private float _minTargetDist = 5f;
    [SerializeField] private bool _isNonPlayableScene;//TODO extremely shitty solution, class must be split up later

    [Inject] private PlayerInputActions _input;

    private List<Engine> _engines = new();
    private Vector3 _target;
    private DisposableBag _disposables;
    private CancellationTokenSource _cts;



    void Start()
    {
        _ship.Parts.CollectionChanged += OnShipChanged;

        Serializer.DeserializeShip(this);
        Serializer.DeserializeInventory(_inventory);


        _cts = new CancellationTokenSource().AddTo(ref _disposables);
        _input.Player.Select.Enable();
        _input.AddTo(ref _disposables);


        if (_isNonPlayableScene)
            return;



        var disposableMouseInput = _input.Player.Select
            .StartedAsObservable(_cts.Token)
            .Select(context => Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>()))
            .Subscribe(point => SetTarget(point))
            .AddTo(ref _disposables);



    }
    void Update()
    {
        if (_isNonPlayableScene)
            return;

        Move();
    }
    void OnDestroy()
    {
        _cts.Cancel();
        _input.Player.Select.Disable();
        _disposables.Dispose();
    }



    private void OnShipChanged(in NotifyCollectionChangedEventArgs<KeyValuePair<Vector2Int, GameObject>> e)
    {
        if (!e.IsSingleItem)
            throw new Exception("Can't handle more than one ship part change at a time yet");

        Engine engine;

        switch (e.Action)
        {
            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                engine = e.NewItem.Value.GetComponent<Engine>();
                if (engine != null)
                    _engines.Add(engine);
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                engine = e.OldItem.Value.GetComponent<Engine>();
                if (engine != null)
                    _engines.Remove(engine);
                break;
            default:
                throw new Exception($"Unexpected action {e.Action} can't be handled yet");
        }
    }
    private void Move()
    {
        foreach (var engine in _engines)
        {
            if (Vector2.Distance(_target, _ship.transform.position) < _minTargetDist)
            {
                engine.ThrustControl = 0f;
                engine.TorqueControl = 0f;
                continue;
            }

            Vector3 dir = (_target - _ship.transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;//-90 is for looking up, not right
            float deltaAngle = Mathf.DeltaAngle(angle, _ship.transform.eulerAngles.z);

            engine.TorqueControl = Mathf.Sign(deltaAngle) * Mathf.Abs(deltaAngle / 180f);
            engine.ThrustControl = 1f - Mathf.Abs(deltaAngle / 180f);
        }
    }
    private void SetTarget(Vector3 target)
    {
        _target = target;
    }
}
