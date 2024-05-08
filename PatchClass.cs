using System;
using HarmonyLib;

namespace ExpeditionTakeoff;

[HarmonyPatch]
public class PatchClass
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(SubmitActionLoadScene), nameof(SubmitActionLoadScene.ConfirmSubmit))]
    public static void ConfirmSubmit_Postfix(SubmitActionLoadScene __instance)
    {
        if (__instance._sceneToLoad == SubmitActionLoadScene.LoadableScenes.GAME
            || __instance._sceneToLoad == SubmitActionLoadScene.LoadableScenes.EYE)
        {
            ExpeditionTakeoff.Instance.StartTakeoffSequence();
        }
    }
}
