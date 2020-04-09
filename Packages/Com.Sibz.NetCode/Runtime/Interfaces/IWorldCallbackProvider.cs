using System;

namespace Sibz.NetCode
{
    public interface IWorldCallbackProvider : IWorldCreatorCallbackProvider
    {
        Action WorldDestroyed { get; set; }
        Action PreWorldDestroy { get; set; }
    }
}