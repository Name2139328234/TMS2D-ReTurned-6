using Reflex.Core;
using UnityEngine;



public class ShipyardSceneInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private AudioSource _buttonEnterSound;



    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterFactory(container => new ButtonAudioStorage(_buttonEnterSound), Reflex.Enums.Lifetime.Scoped, Reflex.Enums.Resolution.Eager);
    }
}
