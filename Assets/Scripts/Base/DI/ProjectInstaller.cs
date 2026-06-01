using Reflex.Core;
using UnityEngine;



public class ProjectInstaller : MonoBehaviour, IInstaller
{
    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterType(typeof(PlayerInputActions), Reflex.Enums.Lifetime.Transient, Reflex.Enums.Resolution.Lazy);
    }
}
