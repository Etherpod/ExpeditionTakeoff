using UnityEngine;

namespace ExpeditionTakeoff
{
    public class AnimEventListener : MonoBehaviour
    {
        public void UnparentFromPlanet()
        {
            ExpeditionTakeoff.Instance.UnparentShipFromPlanet();
        }
    }
}
