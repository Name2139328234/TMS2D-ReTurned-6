using Closures;
using UnityEngine;



public class BuildingSound : MonoBehaviour
{
    [SerializeField] private Builder _builder;
    [SerializeField] private AudioSource _buildSound;
    [SerializeField] private AudioSource _unbuildSound;    


    void Start()
    {
        _builder.OnBuild += Closure.Action<AudioSource, BuildingArgs>(_buildSound, (sound, args) =>
        {
            if (args.IsPriced) sound.Play();
        }).AsAction();
        _builder.OnUnbuild += Closure.Action<AudioSource, BuildingArgs>(_unbuildSound, (sound, args) =>
        {
            if (args.IsPriced) sound.Play();
        }).AsAction();
    }
}
