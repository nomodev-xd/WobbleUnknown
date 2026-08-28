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
            if (camera == null || camera.tradeMenu == null) 
                return;

            if (camera.tradeMenu.activeSelf)
            {
                var trader = camera.currentTrader;
                trader?.GetComponent<TraderWobble>()?.TriggerWobble();
            }
        }
    }
}