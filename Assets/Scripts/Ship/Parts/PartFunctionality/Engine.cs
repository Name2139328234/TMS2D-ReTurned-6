using Cysharp.Threading.Tasks;
using System;
using UnityEngine;



public class Engine : MonoBehaviour
{
    public float ThrustControl { get => _thrustControl; set => _thrustControl = Mathf.Clamp(value, -1f, 1f); }
    public float TorqueControl { get => _torqueControl; set => _torqueControl = Mathf.Clamp(value, -1f, 1f); }

    [SerializeField] private float _thrust;
    [SerializeField] private float _torque;

    private Rigidbody2D _target;
    private bool _isUsed;
    private float _thrustControl;
    private float _torqueControl;




    void Start()
    {
        _target = GetComponentInParent<Rigidbody2D>();
    }
    void Update()
    {
        /*
        if (!_isUsed)
            return;
        //*/

        _target.AddForceAtPosition(_thrust * _thrustControl * transform.up * Time.deltaTime, transform.position, ForceMode2D.Impulse);
        _target.AddTorque(-_torque * _torqueControl * Time.deltaTime, ForceMode2D.Impulse);
    }



    public async UniTask Use(TimeSpan time)
    {
        if(_isUsed)
            return;

        _isUsed = true;
        await UniTask.Delay(time, cancellationToken: gameObject.GetCancellationTokenOnDestroy());
        _isUsed = false;
    }
}
