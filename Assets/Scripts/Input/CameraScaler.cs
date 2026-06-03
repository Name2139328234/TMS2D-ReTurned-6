using R3;
using ReactiveInputSystem;
using Reflex.Attributes;
using System.Threading;
using UnityEngine;



public class CameraScaler : MonoBehaviour
{
    public Camera Camera { get => _camera; }
    public float Min { get => _min; }

    [SerializeField] private Camera _camera;
    [SerializeField] private float _sensitivity;
    [SerializeField] private float _max;
    [SerializeField] private float _min;

    [Inject] private PlayerInputActions _input;

    private DisposableBag _disposables;
    private CancellationTokenSource _cts;

    

    void Start()
    {
        _cts = new CancellationTokenSource().AddTo(ref _disposables);

        _input.Player.Scroll.Enable();
        _input.AddTo(ref _disposables);

        _input.Player.Scroll
            .PerformedAsObservable(_cts.Token)
            .Select(context => context.ReadValue<Vector2>().y * _sensitivity * Time.deltaTime)
            .Subscribe(scroll =>
            {
                _camera.orthographicSize -= scroll;
                _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, _min, _max);
            })
            .AddTo(ref _disposables);
    }
    void OnDestroy()
    {
        _cts.Cancel();
        _input.Player.Scroll.Disable();
        _disposables.Dispose();
    }
}
