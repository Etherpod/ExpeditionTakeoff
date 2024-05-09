using OWML.Common;
using OWML.ModHelper;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace ExpeditionTakeoff;

public class ExpeditionTakeoff : ModBehaviour
{
    public static ExpeditionTakeoff Instance;

    private TitleScreenManager _titleScreenManager;
    private GameObject _shipObject;
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
        GameObject shipPrefab = AssetBundleUtilities.LoadPrefab("assets/expeditiontakeoff", "Assets/ShipAnim/Ship_Pivot.prefab", this);
        Transform shipProxy = GameObject.Find("Structure_HEA_PlayerShip_v4_NearProxy").transform;
        _shipObject = Instantiate(shipPrefab, shipProxy.parent);
        _shipObject.transform.position = shipProxy.position;
        _shipObject.transform.rotation = shipProxy.rotation;
        _shipObject.transform.localScale = shipProxy.localScale;
        shipProxy.gameObject.SetActive(false);
        _shipObject.SetActive(true);
        _shipObject.GetComponentInChildren<ShipThrusterAnimator>().enabled = true;

        _campfireController = FindObjectOfType<Campfire>();
    }

    public void StartTakeoffSequence()
    {
        ModHelper.Console.WriteLine("Started takeoff");
        _titleScreenManager._resumeGameAction.OnSubmitAction -= StartTakeoffSequence;
        _titleScreenManager._newGameAction.OnSubmitAction -= StartTakeoffSequence;

        _campfireController.SetState(Campfire.State.UNLIT);

        StartCoroutine(ShipLiftoffDelay());
    }

    private IEnumerator ShipLiftoffDelay()
    {
        yield return new WaitForSeconds(3f);
        _shipObject.GetComponentInChildren<Animator>().SetTrigger("Liftoff");
    }
}
