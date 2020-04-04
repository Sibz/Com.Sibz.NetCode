using System;

namespace Sibz.NetCode
{
    public interface IWorldCallbackProvider
    {
        Action WorldCreated { get; set; }
        Action WorldDestroyed { get; set; }
        Action PreWorldDestroy { get; set; }
    }
}