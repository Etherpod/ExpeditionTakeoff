using OWML.Common;
using OWML.ModHelper;

namespace ExpeditionTakeoff;

public class ExpeditionTakeoff : ModBehaviour
{
    public static ExpeditionTakeoff Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ModHelper.Console.WriteLine($"My mod {nameof(ExpeditionTakeoff)} is loaded!", MessageType.Success);

        LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
        {

        };
    }

    public void StartTakeoffSequence()
    {

    }
}
