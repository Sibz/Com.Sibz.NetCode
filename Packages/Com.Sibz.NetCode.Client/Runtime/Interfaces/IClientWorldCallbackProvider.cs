using System;

namespace Sibz.NetCode
{
    public interface IClientWorldCallbackProvider
    {
        Action Connecting { get; set; }
        Action Connected { get; set; }
        Action ConnectionFailed { get; set; }
        Action Disconnected { get; set; }
    }
}