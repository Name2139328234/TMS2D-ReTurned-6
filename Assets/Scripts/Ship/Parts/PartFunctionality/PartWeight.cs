using UnityEngine;

public class PartWeight : MonoBehaviour
{
    public float Value { get => _value; }

    [SerializeField] private float _value;
}
