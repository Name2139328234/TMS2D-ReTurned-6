using Cysharp.Threading.Tasks;
using R3;
using ReactiveInputSystem;
using Reflex.Attributes;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;



public class BuilderInput : MonoBehaviour
{
    [SerializeField] private Builder _builder;

    [Inject] private PlayerInputActions _input;

    private bool _isPointerOverObjectLastFrame;//this has to be stored for async access
    private DisposableBag _disposables;
    private CancellationTokenSource _cts;//TODO consider a parent "AbstractInput" class for all the input-related activities in order to not repeat this code all the time



    void Start()
    {
        _cts = new CancellationTokenSource().AddTo(ref _disposables);

        _input.Player.Select.Enable();
        _input.Player.Order.Enable();
        _input.AddTo(ref _disposables);

        var disposableMouseInput = _input.Player.Select
            .StartedAsObservable(_cts.Token)
            .Select(context => Vector2Int.FloorToInt(Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>()) + Vector3.one * 0.5f))
            .Where(point => !_builder.Ship.Parts.ContainsKey(point) && _builder.IsEnough() && !_isPointerOverObjectLastFrame)
            .Subscribe(point =>
            {
                _builder.SelectedPosition = point;
                _builder.Build(true);
            })
            .AddTo(ref _disposables);

        var disposableInputRMB = _input.Player.Order
            .StartedAsObservable(_cts.Token)
            .Select(context => Vector2Int.FloorToInt(Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>()) + Vector3.one * 0.5f))
            .Where(point => _builder.Ship.Parts.ContainsKey(point))
            .Subscribe(point =>
            {
                _builder.SelectedPosition = point;
                _builder.Unbuild(true);
            })
            .AddTo(ref _disposables);
    }
    void Update()
    {
        _isPointerOverObjectLastFrame = EventSystem.current.IsPointerOverGameObject();
    }
    private void OnDestroy()
    {
        _cts.Cancel();//disposing the cts DOESN'T cancel the tokens by itself
        _input.Player.Select.Disable();
        _input.Player.Order.Disable();
        _disposables.Dispose();
    }
}
