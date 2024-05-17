using HarmonyLib;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ExpeditionTakeoff;

[HarmonyPatch]
public class PatchClass
{
    public static bool profileLoaded = false;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Campfire), nameof(Campfire.SetState))]
    public static void Campfire_Postfix(Campfire __instance)
    {
        if (LoadManager.GetCurrentScene() != OWScene.TitleScreen) return;

        if (__instance._state == Campfire.State.UNLIT)
        {
            for (int i = 0; i < __instance._litRenderers.Length; i++)
            {
                __instance._litRenderers[i].SetActivation(true);
            }
        }
    }

    [HarmonyReversePatch]
    [HarmonyPatch(typeof(SubmitActionConfirm), nameof(SubmitActionConfirm.ConfirmSubmit))]
    public static void SubmitActionConfirm_ConfirmSubmit(SubmitActionConfirm instance) { }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SubmitActionLoadScene), nameof(SubmitActionLoadScene.ConfirmSubmit))]
    public static bool GIVE_ME_LOADING_TIME(SubmitActionLoadScene __instance)
    {
        ExpeditionTakeoff.Instance.StartCoroutine(ExpeditionTakeoff.Instance.WaitForProfile(__instance));
        return false;
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TitleScreenManager), nameof(TitleScreenManager.OnProfileManagerReadDone))]
    public static void CheckForLoading(TitleScreenManager __instance)
    {
        profileLoaded = true;
    }
}
