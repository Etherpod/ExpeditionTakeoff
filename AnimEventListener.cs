using UnityEngine;

namespace ExpeditionTakeoff;

public class AnimEventListener : MonoBehaviour
{
    [SerializeField]
    TitleShipAudioController _audioController;

    public void UnparentFromPlanet()
    {
        ExpeditionTakeoff.Instance.UnparentShipFromPlanet();
    }

    public void PlayDamageAudio(int damageLevel)
    {
        _audioController.PlayDamageAudio(damageLevel);
    }
}
