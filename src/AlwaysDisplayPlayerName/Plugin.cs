using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace AlwaysDisplayPlayerName;

[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;

    internal static ConfigEntry<bool> configEnable = null!;
    internal static ConfigEntry<float> configVisibleAngle = null!;
    internal static ConfigEntry<bool> configDisplayWhenBlind = null!;
    internal static ConfigEntry<bool> configShowDistance = null!;

    internal static Plugin Instance { get; private set; } = null!;

    private void Awake()
    {
        Log = Logger;
        Instance = this;
        configEnable = Config.Bind("General", "Enable", true);
        configVisibleAngle = Config.Bind("General", "VisibleAngle", 52f);
        configDisplayWhenBlind = Config.Bind("General", "DisplayWhenBlind", false);
        configShowDistance = Config.Bind("General", "ShowDistance", true);
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        Log.LogInfo($"Plugin {Name} is loaded!");
    }
}