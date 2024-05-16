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
        if (!ExpeditionTakeoff.Instance.longLoadingTime || LoadManager.GetCurrentScene() != OWScene.TitleScreen) return true;
        if (__instance._receivedSubmitAction) return false;
        SubmitActionConfirm_ConfirmSubmit(__instance);
        __instance._receivedSubmitAction = true;
        Locator.GetMenuInputModule().DisableInputs();
        ExpeditionTakeoff.Instance.StartCoroutine(ExpeditionTakeoff.Instance.LoadDelay(__instance));
        return false;
    }
}
