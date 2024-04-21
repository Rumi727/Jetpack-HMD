using GameNetcodeStuff;
using HarmonyLib;

namespace Rumi.JetpackHMD
{
    class JetpackHMDPatches
    {
        public static PlayerControllerB? playerControllerB;

        [HarmonyPatch(typeof(PlayerControllerB), "Awake")]
        [HarmonyPostfix]
        static void PlayerControllerB_Awake_Postfix(ref PlayerControllerB __instance) => playerControllerB = __instance;

        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPostfix]
        static void PlayerControllerB_Update_Postfix(ref PlayerControllerB __instance) => playerControllerB = __instance;
    }
}
