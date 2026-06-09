using Closures;
using Reflex.Core;
using UnityEngine;



public class PlanetSceneInstaller : MonoBehaviour, IInstaller
{
    [SerializeField] private PlanetGenerator _planetGenerator;



    public void InstallBindings(ContainerBuilder containerBuilder)
    {
        containerBuilder.RegisterFactory<IPositioner>(Closure.Func<PlanetGenerator, Container, PlanetPositioner>(_planetGenerator, (generator, container) => new PlanetPositioner(_planetGenerator)).AsFunc(), new[] { typeof(IPositioner) }, Reflex.Enums.Lifetime.Scoped, Reflex.Enums.Resolution.Lazy);
    }
}
