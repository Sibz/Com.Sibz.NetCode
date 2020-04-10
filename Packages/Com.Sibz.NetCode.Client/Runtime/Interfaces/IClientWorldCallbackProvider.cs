using System;

namespace Sibz.NetCode
{
    public interface IClientWorldCallbackProvider
    {
        Action Connecting { get; set; }
        Action<int> Connected { get; set; }
        Action<string> ConnectionFailed { get; set; }
        Action Disconnected { get; set; }
    }
}