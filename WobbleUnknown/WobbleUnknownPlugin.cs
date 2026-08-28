using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace WobbleUnknown
{
    [BepInPlugin(PluginInfo.Guid, PluginInfo.Name, PluginInfo.Version)]
    public sealed class WobbleUnknownPlugin : BaseUnityPlugin
    {
        public static WobbleUnknownPlugin Instance { get; private set; }
        public ManualLogSource Log => Logger;

        private readonly Harmony _harmony = new Harmony(PluginInfo.Guid);

        private void Awake()
        {
            Instance = this;
            SoundLoader.LoadAllEmbeddedSounds();

            _harmony.PatchAll();
            Log.LogInfo($"Plugin {PluginInfo.Name} {PluginInfo.Version} loaded.");
        }

        private void OnDestroy()
        {
            _harmony.UnpatchSelf();

            Instance = null;
        }
    }
}