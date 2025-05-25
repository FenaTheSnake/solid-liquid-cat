using UnityEngine;
using System.Collections.Generic;
using Zenject;

public class Surfaces : IInitializable
{
    List<StickySurface> _stickySurfaces;

    public void Initialize()
    {
        //_stickySurfaces = new List<StickySurface>();
    }

    public void RegisterStickySurface(StickySurface surface)
    {
        if(_stickySurfaces == null) _stickySurfaces = new List<StickySurface>();
        _stickySurfaces.Add(surface);
    }
    public void UnregisterStickySurface(StickySurface surface)
    {
        _stickySurfaces.Remove(surface);
    }

    public void UnstickAllVerticesFromAllSurfaces()
    {
        if (_stickySurfaces == null) return;
        foreach (var surface in _stickySurfaces)
        {
            surface.UnstickFromAllVertices();
        }
    }

}
