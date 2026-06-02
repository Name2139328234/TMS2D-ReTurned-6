using R3;
using UnityEngine;



public class EnergyUser : MonoBehaviour
{
    public ReactiveProperty<bool> IsOn { get => _isOn; }
    public float UseAmount { get => _useAmount; }

    [SerializeField] private float _useAmount;
    [SerializeField] private float _maxBuffer;

    private ReactiveProperty<bool> _isOn = new();
    private float _buffer;//I thought of making this into ReactiveProperty<float>, but all implementations were overenginered and less effective



    void Update()
    {
        _buffer -= _useAmount * Time.deltaTime;
        CheckOn();
    }



    public void GiveEnergy(float energy)
    {
        _buffer += energy;
        CheckOn();
    }



    private void CheckOn()
    {
        _buffer = Mathf.Clamp(_buffer, 0f, _maxBuffer);
        _isOn.Value = _buffer > 0f;
    }
}
