using System;
using HarmonyLib;

namespace ExpeditionTakeoff;

[HarmonyPatch]
public class PatchClass
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Campfire), nameof(Campfire.SetState))]
    public static void Campfire_Postfix(Campfire __instance)
    {
        if (LoadManager.GetCurrentScene() == OWScene.TitleScreen && __instance._state == Campfire.State.UNLIT)
        {
            ExpeditionTakeoff.Instance.ModHelper.Console.WriteLine("Correct state");
            for (int i = 0; i < __instance._litRenderers.Length; i++)
            {
                __instance._litRenderers[i].SetActivation(true);
            }
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(SubmitActionLoadScene), nameof(SubmitActionLoadScene.ConfirmSubmit))]
    public static bool GIVE_ME_LOADING_TIME()
    {
        if (ExpeditionTakeoff.Instance.infiniteLoadingTime)
        {
            ExpeditionTakeoff.Instance.StartTakeoffSequence();
            return false;
        }

        return true;
    }
}
