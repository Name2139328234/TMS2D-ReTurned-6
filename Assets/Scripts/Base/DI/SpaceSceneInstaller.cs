using Closures;
using Reflex.Core;
using UnityEngine;



public class SpaceSceneInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private Bounds _positionerBounds;

    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterFactory<IPositioner>(Closure.Func<Bounds, Container, SpacePositioner>(_positionerBounds, (bounds, container) => new SpacePositioner(bounds)).AsFunc(), new[] { typeof(IPositioner) }, Reflex.Enums.Lifetime.Scoped, Reflex.Enums.Resolution.Lazy);
    }
}
