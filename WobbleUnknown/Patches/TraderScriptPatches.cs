using HarmonyLib;
// ReSharper disable InconsistentNaming

namespace WobbleUnknown.Patches
{
    [HarmonyPatch]
    public static class TraderScriptPatches
    {
        [HarmonyPatch(typeof(TraderScript), "Start")]
        [HarmonyPostfix]
        private static void TraderScriptStartPostfix(TraderScript __instance)
        {
            if (__instance.GetComponent<TraderWobble>() == null)
                __instance.gameObject.AddComponent<TraderWobble>();
        }

        [HarmonyPatch(typeof(TraderScript), "OnWillRenderObject")]
        [HarmonyPostfix]
        private static void TraderScriptOnWillRenderObjectPostfix(TraderScript __instance)
        {
            __instance.GetComponent<TraderWobble>()?.ApplyWobble();
        }
    }
}