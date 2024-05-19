using HarmonyLib;

namespace ExpeditionTakeoff;

[HarmonyPatch]
public class PatchClass
{
    public static bool profileLoaded = false;
    public static bool firstLoadAttempt = false;

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

    [HarmonyReversePatch]
    [HarmonyPatch(typeof(SubmitActionLoadScene), nameof(SubmitActionLoadScene.ConfirmSubmit))]
    public static void SubmitActionLoadScene_ConfirmSubmit(SubmitActionLoadScene instance) { }

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
