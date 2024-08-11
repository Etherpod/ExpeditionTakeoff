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
    private SubmitActionLoadScene _selectedButton;
    private GameObject _shipObject;
    private TitleShipAudioController _shipAudioController;
    private Campfire _campfireController;
    private TravelerController _traveler;
    private AssetBundle _shipBundle;
    private float loadTime;
    public float sfxVolume;

    public static bool DebugModeEnabled = true;

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
        _traveler = FindObjectOfType<TravelerController>();
        quickLoad = (!titleScreenAnimation._introPan || titleScreenAnimation._fadeDuration == 0);

        if (quickLoad && _traveler)
        {
            _traveler.gameObject.SetActive(false);
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
        GameObject shipPrefab = (GameObject)_shipBundle.LoadAsset("Assets/ShipAnim/Ship_Pivot.prefab");
        AssetBundleUtilities.ReplaceShaders(shipPrefab);

        Transform shipProxy = GameObject.Find("Structure_HEA_PlayerShip_v4_NearProxy").transform;
        if (shipProxy)
        {
            _shipObject = Instantiate(shipPrefab, shipProxy.parent);
            _shipObject.transform.position = shipProxy.position;
            _shipObject.transform.rotation = shipProxy.rotation;
            _shipObject.transform.localScale = shipProxy.localScale;
            shipProxy.gameObject.SetActive(false);
            _shipObject.GetComponentInChildren<ShipThrusterAnimator>().enabled = true;
            _shipAudioController = _shipObject.GetComponentInChildren<TitleShipAudioController>();
        }

        _campfireController = FindObjectOfType<Campfire>();
    }

    public void StartTakeoffSequence()
    {
        if (_selectedButton)
        {
            _selectedButton.OnSubmitAction -= StartTakeoffSequence;
        }

        if (_campfireController)
        {
            _campfireController.SetState(Campfire.State.UNLIT);
        }

        StartCoroutine(ShipLiftoffDelay());
    }

    private IEnumerator ShipLiftoffDelay()
    {
        yield return new WaitForSeconds(1f);
        if (!quickLoad && _traveler)
        {
            _traveler.gameObject.SetActive(false);
        }
        if (_campfireController)
        {
            _campfireController.SetState(Campfire.State.SMOLDERING);
        }
        quickLoad = false;

        yield return new WaitForSeconds(1.1f);

        if (_shipObject)
        {
            _shipAudioController.PlayIgnitionAudio();
        }

        yield return new WaitForSeconds(0.9f);

        if (_shipObject)
        {
            _shipObject.GetComponentInChildren<Animator>().SetInteger("LiftoffIndex", Random.Range(0, 5));
            _shipAudioController.PlayLoopingAudio();
        }
    }

    public void UnparentShipFromPlanet()
    {
        _shipObject.transform.parent = null;
    }

    public IEnumerator WaitForProfile(SubmitActionLoadScene __instance)
    {
        yield return new WaitUntil(() => PatchClass.profileLoaded);
        hasGameSave = _titleScreenManager._profileManager.currentProfileGameSave.loopCount > 1;
        if (quickLoad && !hasGameSave && !PatchClass.firstLoadAttempt)
        {
            PatchClass.firstLoadAttempt = true;
            yield break;
        }
        if (!longLoadingTime || (quickLoad && hasGameSave) || LoadManager.GetCurrentScene() != OWScene.TitleScreen 
                || __instance._sceneToLoad == SubmitActionLoadScene.LoadableScenes.CREDITS || __instance._sceneToLoad == SubmitActionLoadScene.LoadableScenes.TITLE)
        {
            PatchClass.SubmitActionLoadScene_ConfirmSubmit(__instance);
            yield break;
        }
        if (__instance._receivedSubmitAction)
        {
            yield break;
        }
        __instance.OnSubmitAction += StartTakeoffSequence;
        _selectedButton = __instance;
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
        sfxVolume = config.GetSettingsValue<float>("sfxVolume") / 2;
    }

    public static void WriteDebugMessage(object msg)
    {
        if (DebugModeEnabled)
        {
            Instance.ModHelper.Console.WriteLine(msg.ToString());
        }
    }
}
