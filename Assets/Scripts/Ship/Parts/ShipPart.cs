using UnityEngine;

public class ShipPart : MonoBehaviour
{
    public PartInfo Info { get => _info; }

    [SerializeField] private PartInfo _info;
}
