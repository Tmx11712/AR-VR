#if UNITY_EDITOR_WIN
using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;

// Unity 6.3 on Windows does not always honor isPreloaded on package DLLs, causing Vuforia DRIVER_CONFIG_LOAD_ERROR.
[InitializeOnLoad]
public static class VuforiaDriverPreloader
{
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr LoadLibrary(string lpFileName);

    static VuforiaDriverPreloader()
    {
        Preload();
        EditorApplication.playModeStateChanged += state =>
        {
            if (state == PlayModeStateChange.ExitingEditMode)
                Preload();
        };
    }

    private static void Preload()
    {
        var pluginDir = Path.GetFullPath("Packages/com.ptc.vuforia.engine/Vuforia/Plugins/Windows/x64");
        if (!Directory.Exists(pluginDir)) return;

        foreach (var dll in new[] { "UnityDriver.dll", "FileDriver.dll" })
        {
            var fullPath = Path.Combine(pluginDir, dll);
            if (File.Exists(fullPath))
                LoadLibrary(fullPath);
        }
    }
}
#endif
