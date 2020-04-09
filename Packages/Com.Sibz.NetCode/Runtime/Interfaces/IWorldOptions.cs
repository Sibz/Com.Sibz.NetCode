namespace Sibz.NetCode
{
    public interface IWorldOptions : IWorldCreatorOptions
    {
        bool CreateWorldOnInstantiate { get; }
    }
}