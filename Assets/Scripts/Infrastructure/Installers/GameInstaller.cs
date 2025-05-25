using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<GameState>().AsSingle();
        Container.Bind<Surfaces>().AsSingle().NonLazy();
    }
}
