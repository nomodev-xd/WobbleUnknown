using BepInEx;
using HarmonyLib;

namespace WobbleUnknown
{
    [BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
    public sealed class WobbleUnknownPlugin : BaseUnityPlugin
    {
        private readonly Harmony _harmony = new Harmony(PluginInfo.Guid);

        private void Awake()
        {
            _harmony.PatchAll();
            Logger.LogInfo($"Plugin {PluginInfo.Name} {PluginInfo.Version} loaded.");
        }

        private void OnDestroy()
        {
            _harmony.UnpatchSelf();
        }
    }
}