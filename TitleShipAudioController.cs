using UnityEngine;

namespace ExpeditionTakeoff;

public class TitleShipAudioController : MonoBehaviour
{
    [SerializeField]
    private OWAudioSource _oneShotAudio;
    [SerializeField]
    private OWAudioSource _loopingAudio;

    private ShipThrusterAnimator _thrusterAnimator;
    private bool _playThrustAudio = false;
    
    private void Start()
    {
        _thrusterAnimator = transform.parent.GetComponentInChildren<ShipThrusterAnimator>();
        _loopingAudio.AssignAudioLibraryClip(AudioType.ShipThrustTranslational_LP);
    }

    private void Update()
    {
        _loopingAudio.SetLocalVolume(_thrusterAnimator.GetThrustMagnitude());
    }

    public void PlayIgnitionAudio()
    {
        _oneShotAudio.PlayOneShot(AudioType.ShipThrustIgnition, ExpeditionTakeoff.Instance.sfxVolume);
    }

    public void PlayLoopingAudio()
    {
        _loopingAudio.FadeIn(0.5f, true, targetVolume: ExpeditionTakeoff.Instance.sfxVolume);
    }

    public void StopLoopingAudio()
    {
        _loopingAudio.FadeOut(0.5f);
    }

    public void PlayDamageAudio(int damageLevel)
    {
        float volume = Random.Range(0.8f, 1f) * ExpeditionTakeoff.Instance.sfxVolume;

        if (damageLevel >= 2)
        {
            _oneShotAudio.PlayOneShot(AudioType.ShipImpact_HeavyDamage, volume);
        }
        else if (damageLevel == 1)
        {
            _oneShotAudio.PlayOneShot(AudioType.ShipImpact_MediumDamage, volume);
        }
        else
        {
            _oneShotAudio.PlayOneShot(AudioType.ShipImpact_LightDamage, volume);
        }
    }
}
