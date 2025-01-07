using System;

namespace Services.Abstractions;

public interface IMapGenerator
{
    public event Action MapGenerated;
}