using System;

namespace Sibz.NetCode
{
    public interface IWorldCreatorCallbackProvider
    {
        Action WorldCreated { get; set; }
    }
}