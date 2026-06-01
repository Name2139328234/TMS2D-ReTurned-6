using UnityEngine;



public class EnergyGenerator : MonoBehaviour
{
    public float Production { get => _production; set => _production = value; }

    [SerializeField] private float _production;
}
