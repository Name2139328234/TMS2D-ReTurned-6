using R3;
using ReactiveInputSystem;
using Reflex.Attributes;
using System.Threading;
using UnityEngine;



public class ViewMover : MonoBehaviour
{
    [SerializeField] private Bounds _bounds;
    [SerializeField] private float _sensitivity;

    [Inject] private PlayerInputActions _input;

    private DisposableBag _disposables;
    private CancellationTokenSource _cts;



    void Start()
    {
        _cts = new CancellationTokenSource().AddTo(ref _disposables);

        _input.Enable();
        _input.AddTo(ref _disposables);

        _input.Player.Stride
            .PerformedAsObservable(_cts.Token)
            .Select(context => _sensitivity * Time.deltaTime * context.ReadValue<Vector2>())
            .Subscribe(delta =>
            {
                Vector3 pos = transform.position;
                pos -= new Vector3(delta.x, delta.y);
                pos.x = Mathf.Clamp(pos.x, _bounds.min.x, _bounds.max.x);
                pos.y = Mathf.Clamp(pos.y, _bounds.min.y, _bounds.max.y);
                transform.position = pos;
            })
            .AddTo(ref _disposables);
    }
    void OnDestroy()
    {
        _cts.Cancel();
        _input.Disable();
        _disposables.Dispose();
    }
}
