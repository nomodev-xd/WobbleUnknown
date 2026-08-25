using HarmonyLib;

namespace WobbleUnknown.Patches
{
    [HarmonyPatch]
    public static class PlayerCameraPatches
    {
        [HarmonyPatch(typeof(PlayerCamera), "ToggleTradeMenu")]
        [HarmonyPostfix]
        private static void ToggleTradeMenuPostfix()
        {
            var camera = PlayerCamera.main;
            var trader = camera != null ? camera.currentTrader : null;
            trader?.GetComponent<TraderWobble>()?.TriggerWobble();
        }
    }
}