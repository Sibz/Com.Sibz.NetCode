namespace Sibz
{
    public static partial class Util
    {
#if DEBUG
        public static int DebugLevel = 1;

        public static void Debug(string message, int level = 1)
        {
            if (level <= DebugLevel)
            {
                UnityEngine.Debug.Log(message);
            }
        }
#endif
    }
}