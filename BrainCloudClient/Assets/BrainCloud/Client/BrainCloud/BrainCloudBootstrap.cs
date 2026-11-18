using System;
#if !UNITY_5_3_OR_NEWER && !NET5_0_OR_GREATER
using System.Reflection;
#endif

namespace BrainCloud
{
    /// <summary>
    /// Receives the build's generated tuning file, which registers itself at startup.
    /// Apps don't call this directly.
    /// </summary>
    public static class BrainCloudBootstrap
    {
        private static Action<byte[]> _fill;

        public static void Use(Action<byte[]> fill)
        {
            _fill = fill;
        }

        // Null when the app has none.
        internal static byte[] Read()
        {
#if !UNITY_5_3_OR_NEWER && !NET5_0_OR_GREATER
            if (_fill == null)
                FindGenerated();
#endif
            if (_fill == null)
                return null;

            var value = new byte[32];
            _fill(value);
            return value;
        }

        internal static void Wipe(byte[] value)
        {
            if (value != null)
                Array.Clear(value, 0, value.Length);
        }

#if !UNITY_5_3_OR_NEWER && !NET5_0_OR_GREATER
        // Targets without module initializers: the generated file adds a fixed-name entry for these.
        private static void FindGenerated()
        {
            try
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var type = assembly.GetType("BrainCloudTuning", false);
                    var use = type == null ? null : type.GetMethod("Use", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (use != null)
                    {
                        use.Invoke(null, null);
                        return;
                    }
                }
            }
            catch (Exception)
            {
                // Treated as no tuning value.
            }
        }
#endif
    }
}
