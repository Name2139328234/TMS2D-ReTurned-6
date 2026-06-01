using UnityEngine;



[CreateAssetMenu(fileName = "BuilderBlocks", menuName = "Custom/BuilderBlocks", order = 0)]
public class BuilderBlocks : ScriptableObject
{
    public PartPriceInfo[] AvailableParts { get => _availableParts; }

    [SerializeField] private PartPriceInfo[] _availableParts;
}
