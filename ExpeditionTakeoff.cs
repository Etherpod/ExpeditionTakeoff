using OWML.Common;
using OWML.ModHelper;
using System.Collections;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace ExpeditionTakeoff;

public class ExpeditionTakeoff : ModBehaviour
{
    public static ExpeditionTakeoff Instance;
    public bool quickLoad { get; private set; }
    public bool hasGameSave { get; private set; }
    internal bool longLoadingTime;

    private TitleScreenManager _titleScreenManager;
    private GameObject _shipObject;
    private Campfire _campfireController;
    private AssetBundle _shipBundle;
    private float loadTime;

    private void Awake()
    {
        Instance = this;
        HarmonyLib.Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }

    private void Start()
    {
        string shipPath = Path.Combine(ModHelper.Manifest.ModFolderPath, "assets/expeditiontakeoff");
        _shipBundle = AssetBundle.LoadFromFile(shipPath);

        longLoadingTime = ModHelper.Config.GetSettingsValue<bool>("extendLoadingTime");
        loadTime = ModHelper.Config.GetSettingsValue<float>("loadTime");

        TitleScreenAnimation titleScreenAnimation = FindObjectOfType<TitleScreenAnimation>();
        _titleScreenManager = FindObjectOfType<TitleScreenManager>();
        quickLoad = (!titleScreenAnimation._introPan || titleScreenAnimation._fadeDuration == 0);
        if (quickLoad)
        {
            FindObjectOfType<TravelerController>().gameObject.SetActive(false);
        }

        InitObjects();

        LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
        {
            if (loadScene == OWScene.TitleScreen)
            {
                _titleScreenManager = FindObjectOfType<TitleScreenManager>();
                InitObjects();
            }
        };
    }

    private void InitObjects()
    {
        _titleScreenManager._resumeGameAction.OnSubmitAction += StartTakeoffSequence;
        _titleScreenManager._newGameAction.OnSubmitAction += StartTakeoffSequence;

        GameObject shipPrefab = (GameObject)_shipBundle.LoadAsset("Assets/ShipAnim/Ship_Pivot.prefab");
        AssetBundleUtilities.ReplaceShaders(shipPrefab);

        Transform shipProxy = GameObject.Find("Structure_HEA_PlayerShip_v4_NearProxy").transform;
        _shipObject = Instantiate(shipPrefab, shipProxy.parent);
        _shipObject.transform.position = shipProxy.position;
        _shipObject.transform.rotation = shipProxy.rotation;
        _shipObject.transform.localScale = shipProxy.localScale;
        shipProxy.gameObject.SetActive(false);
        _shipObject.GetComponentInChildren<ShipThrusterAnimator>().enabled = true;

        _campfireController = FindObjectOfType<Campfire>();
    }

    public void StartTakeoffSequence()
    {
        _titleScreenManager._resumeGameAction.OnSubmitAction -= StartTakeoffSequence;
        _titleScreenManager._newGameAction.OnSubmitAction -= StartTakeoffSequence;

        _campfireController.SetState(Campfire.State.UNLIT);

        StartCoroutine(ShipLiftoffDelay());
    }

    public void ListenToResumeSubmitAction()
    {
        _titleScreenManager._resumeGameAction.OnSubmitAction += StartTakeoffSequence;
    }

    private IEnumerator ShipLiftoffDelay()
    {
        yield return new WaitForSeconds(1f);
        if (!quickLoad)
        {
            FindObjectOfType<TravelerController>().gameObject.SetActive(false);
        }
        _campfireController.SetState(Campfire.State.SMOLDERING);
        quickLoad = false;
        yield return new WaitForSeconds(2f);
        _shipObject.GetComponentInChildren<Animator>().SetInteger("LiftoffIndex", Random.Range(0, 4));
    }

    public void UnparentShipFromPlanet()
    {
        _shipObject.transform.parent = null;
    }

    public IEnumerator WaitForProfile(SubmitActionLoadScene __instance)
    {
        yield return new WaitUntil(() => PatchClass.profileLoaded);
        ModHelper.Console.WriteLine("profile loaded");
        hasGameSave = _titleScreenManager._profileManager.currentProfileGameSave.loopCount > 1;
        if (quickLoad && !hasGameSave && !PatchClass.firstLoadAttempt)
        {
            PatchClass.firstLoadAttempt = true;
            yield break;
        }
        if (!longLoadingTime || (quickLoad && hasGameSave) || LoadManager.GetCurrentScene() != OWScene.TitleScreen)
        {
            PatchClass.SubmitActionLoadScene_ConfirmSubmit(__instance);
            yield break;
        }
        if (__instance._receivedSubmitAction)
        {
            yield break;
        }
        PatchClass.SubmitActionConfirm_ConfirmSubmit(__instance);
        __instance._receivedSubmitAction = true;
        Locator.GetMenuInputModule().DisableInputs();
        StartCoroutine(LoadDelay(__instance));
        yield break;
    }

    public IEnumerator LoadDelay(SubmitActionLoadScene __instance)
    {
        __instance.ResetStringBuilder();
        __instance._nowLoadingSB.Append(UITextLibrary.GetString(UITextType.LoadingMessage));
        __instance._nowLoadingSB.Append(0.ToString("P0"));
        __instance._loadingText.text = __instance._nowLoadingSB.ToString();
        yield return new WaitForSeconds(loadTime);
        switch (__instance._sceneToLoad)
        {
            case SubmitActionLoadScene.LoadableScenes.GAME:
                LoadManager.LoadSceneAsync(OWScene.SolarSystem, false, LoadManager.FadeType.ToBlack, 1f, false);
                __instance.ResetStringBuilder();
                __instance._waitingOnStreaming = true;
                break;
            case SubmitActionLoadScene.LoadableScenes.EYE:
                LoadManager.LoadSceneAsync(OWScene.EyeOfTheUniverse, true, LoadManager.FadeType.ToBlack, 1f, false);
                __instance.ResetStringBuilder();
                break;
            case SubmitActionLoadScene.LoadableScenes.TITLE:
                LoadManager.LoadScene(OWScene.TitleScreen, LoadManager.FadeType.ToBlack, 2f, true);
                break;
            case SubmitActionLoadScene.LoadableScenes.CREDITS:
                LoadManager.LoadScene(OWScene.Credits_Fast, LoadManager.FadeType.ToBlack, 1f, false);
                break;
        }
    }

    public override void Configure(IModConfig config)
    {
        longLoadingTime = config.GetSettingsValue<bool>("extendLoadingTime");
        loadTime = config.GetSettingsValue<float>("loadTime");
    }
}
