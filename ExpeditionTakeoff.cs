using OWML.Common;
using OWML.ModHelper;
using System.Reflection;
using UnityEngine;

namespace ExpeditionTakeoff;

public class ExpeditionTakeoff : ModBehaviour
{
    public static ExpeditionTakeoff Instance;

    private TitleScreenManager _titleScreenManager;
    private GameObject _thrustersParent;
    private Campfire _campfireController;

    private void Awake()
    {
        Instance = this;
        HarmonyLib.Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }

    private void Start()
    {
        ModHelper.Console.WriteLine($"My mod {nameof(ExpeditionTakeoff)} is loaded!", MessageType.Success);

        ModHelper.Console.WriteLine("Loaded thingy");
        _titleScreenManager = FindObjectOfType<TitleScreenManager>();
        _titleScreenManager._resumeGameAction.OnSubmitAction += StartTakeoffSequence;
        _titleScreenManager._newGameAction.OnSubmitAction += StartTakeoffSequence;
        InitObjects();
    }

    private void Update()
    {

    }

    private void InitObjects()
    {
        ModHelper.Console.WriteLine("Init");
        GameObject thrustersPrefab = AssetBundleUtilities.LoadPrefab("assets/expeditiontakeoff", "Assets/EtherAssets/ShipAnim/Thrusters.prefab", this);
        Transform shipProxyParent = GameObject.Find("Structure_HEA_PlayerShip_v4_NearProxy").transform;
        _thrustersParent = Instantiate(thrustersPrefab, shipProxyParent);
        _thrustersParent.transform.position += new Vector3(0f, 3.85f, 0f);

        _campfireController = FindObjectOfType<Campfire>();
    }

    public void StartTakeoffSequence()
    {
        ModHelper.Console.WriteLine("Started takeoff");
        _titleScreenManager._resumeGameAction.OnSubmitAction -= StartTakeoffSequence;
        _titleScreenManager._newGameAction.OnSubmitAction -= StartTakeoffSequence;

        _campfireController.SetState(Campfire.State.UNLIT);

        foreach (ThrusterFlameController flameController in _thrustersParent.GetComponentsInChildren<ThrusterFlameController>())
        {
            flameController.OnStartTranslationalThrust();
        }
    }
}
