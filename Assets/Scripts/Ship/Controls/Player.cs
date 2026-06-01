using Cysharp.Threading.Tasks;
using ObservableCollections;
using R3;
using ReactiveInputSystem;
using Reflex.Attributes;
using System;
using System.Collections.Generic;
using UnityEngine;



public class Player : MonoBehaviour
{
    public Ship Ship { get => _ship; }
    public Builder Builder { get => _builder; }
    public Inventory Inventory { get => _inventory; }

    [SerializeField] private Ship _ship;
    [SerializeField] private Builder _builder;
    [SerializeField] private Inventory _inventory;

    private List<Engine> _engines = new();
    private Serializer _serializer;
    [Inject] private PlayerInputActions _input;
    private DisposableBag _disposables = new();
    private Vector3 _target;



    void Start()
    {
        _input.Enable();

        _ship.Parts.CollectionChanged += OnShipChanged;

        var disposableMouseInput = _input.Player.Select
            .StartedAsObservable(gameObject.GetCancellationTokenOnDestroy())
            .Select(context => Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>()))
            .Subscribe(point => _target = point)
            .AddTo(ref _disposables);

        var disposableSaveInput = _input.Player.Save
            .StartedAsObservable(gameObject.GetCancellationTokenOnDestroy())
            .Subscribe(context =>
            {
                _serializer.SerializeShip(_ship);
                _serializer.SerializeInventory(_inventory);
            })
            .AddTo(ref _disposables);

        _disposables.AddTo(this);

        //TODO make proper system for this

        _serializer = FindAnyObjectByType<Serializer>();
        _serializer.DeserializeShip(this);
        _serializer.DeserializeInventory(_inventory);
    }
    void Update()
    {
        Move();
    }



    private void OnShipChanged(in NotifyCollectionChangedEventArgs<KeyValuePair<Vector2Int, GameObject>> e)
    {
        if (!e.IsSingleItem)
            throw new Exception("Can't handle more than one ship part change at a time yet");

        var engine = e.NewItem.Value.GetComponent<Engine>();

        switch (e.Action)
        {
            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                if (engine != null)
                    _engines.Add(engine);
                break;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
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
            if (Vector2.Distance(_target, _ship.transform.position) < 5f)
            {
                engine.ThrustControl = 0f;
                engine.TorqueControl = 0f;
                return;
            }

            Vector3 dir = (_target - _ship.transform.position).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
            float deltaAngle = Mathf.DeltaAngle(angle, _ship.transform.eulerAngles.z);

            engine.TorqueControl = Mathf.Sign(deltaAngle) * Mathf.Abs(deltaAngle / 180f);
            engine.ThrustControl = 1 - Mathf.Abs(deltaAngle / 180f);
        }
    }
}
