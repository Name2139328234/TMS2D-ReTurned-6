using UnityEngine;



public class BuildingSound : MonoBehaviour
{
    [SerializeField] private Builder _builder;
    [SerializeField] private AudioSource _buildSound;
    [SerializeField] private AudioSource _unbuildSound;    


    void Start()
    {
        _builder.OnBuild += args => { if (args.IsPriced) _buildSound.Play(); };
        _builder.OnUnbuild += args => { if (args.IsPriced) _unbuildSound.Play(); };
    }
}
