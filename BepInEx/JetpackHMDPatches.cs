using GameNetcodeStuff;
using HarmonyLib;

namespace Rumi.JetpackHMD
{
    class JetpackHMDPatches
    {
        public static PlayerControllerB? playerControllerB;

        [HarmonyPatch(typeof(PlayerControllerB), "Awake")]
        [HarmonyPostfix]
        static void PlayerControllerB_Awake_Postfix(ref PlayerControllerB __instance)
        {
            //조건문 출처: https://github.com/Hamunii/JetpackWarning/blob/main/Patches.cs
            if (__instance.IsOwner && (!__instance.IsServer || __instance.isHostPlayerObject) && __instance.isPlayerControlled && !__instance.isPlayerDead)
                playerControllerB = __instance;
        }

        [HarmonyPatch(typeof(PlayerControllerB), "Update")]
        [HarmonyPostfix]
        static void PlayerControllerB_Update_Postfix(ref PlayerControllerB __instance)
        {
            //조건문 출처: https://github.com/Hamunii/JetpackWarning/blob/main/Patches.cs
            if (__instance.IsOwner && (!__instance.IsServer || __instance.isHostPlayerObject) && __instance.isPlayerControlled && !__instance.isPlayerDead)
                playerControllerB = __instance;
        }
    }
}
