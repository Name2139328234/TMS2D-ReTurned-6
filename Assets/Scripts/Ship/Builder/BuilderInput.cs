using Cysharp.Threading.Tasks;
using R3;
using ReactiveInputSystem;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.EventSystems;



public class BuilderInput : MonoBehaviour
{
    [SerializeField] private Builder _builder;

    [Inject] private PlayerInputActions _input;

    private DisposableBag _disposables;
    private bool _isPointerOverObjectLastFrame;



    void Start()
    {
        _input.Enable();
        _input.AddTo(ref _disposables);

        var disposableMouseInput = _input.Player.Select
            .StartedAsObservable(gameObject.GetCancellationTokenOnDestroy())
            .Select(context => Vector2Int.FloorToInt(Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>()) + Vector3.one * 0.5f))
            .Where(point => !_builder.Ship.Parts.ContainsKey(point) && _builder.IsEnough() && !_isPointerOverObjectLastFrame)
            .Subscribe(point =>
            {
                _builder.SelectedPosition = point;
                _builder.Build(true);
            })
            .AddTo(ref _disposables);

        var disposableInputRMB = _input.Player.Order
            .StartedAsObservable(gameObject.GetCancellationTokenOnDestroy())
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
        _input.Disable();
        _disposables.Dispose();
    }
}
