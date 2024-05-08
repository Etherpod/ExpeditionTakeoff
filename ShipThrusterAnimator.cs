using UnityEngine;

namespace ExpeditionTakeoff
{
    [ExecuteInEditMode]
    public class ShipThrusterAnimator : MonoBehaviour
    {
        [SerializeField]
        private ThrusterFlameController[] _targetThrusters;

        [SerializeField]
        [Range(0f, 1f)]
        private float[] thrusterValues;

        private void Update()
        {
            if (_targetThrusters.Length == 0 || thrusterValues.Length != _targetThrusters.Length)
            {
                Debug.LogError("Check the thruster/value lists, something is wrong with them.");
                return;
            }
            for (int i = 0; i < _targetThrusters.Length; i++)
            {
                _targetThrusters[i].transform.localScale = Vector3.one * thrusterValues[i];
            }
        }
    }
}
