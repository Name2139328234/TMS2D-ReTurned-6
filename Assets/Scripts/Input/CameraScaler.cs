using Closures;
using R3;
using ReactiveInputSystem;
using Reflex.Attributes;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;



public class CameraScaler : MonoBehaviour
{
    public Camera Camera { get => _camera; }
    public float Min { get => _min; }

    [SerializeField] private Camera _camera;
    [SerializeField] private float _sensitivity;
    [SerializeField] private float _min;
    [SerializeField] private float _max;

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
            .Subscribe(DoScroll)
            .AddTo(ref _disposables);
    }
    void OnDestroy()
    {
        _cts.Cancel();
        _input.Player.Scroll.Disable();
        _disposables.Dispose();
    }



    private void DoScroll(InputAction.CallbackContext context)
    {
        float scroll = context.ReadValue<Vector2>().y * _sensitivity * Time.deltaTime;
        _camera.orthographicSize -= scroll;
        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize, _min, _max);
    }



    private struct CameraScrollData
    {
        public Camera Cam;
        public float Scroll;
        public float Min;
        public float Max;



        public CameraScrollData(Camera cam, float scroll, float min, float max)
        {
            Cam = cam;
            Scroll = scroll;
            Min = min;
            Max = max;
        }
    }
}
