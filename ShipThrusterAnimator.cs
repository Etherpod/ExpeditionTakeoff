using UnityEngine;

namespace ExpeditionTakeoff
{
    [ExecuteInEditMode]
    public class ShipThrusterAnimator : MonoBehaviour
    {
        [SerializeField]
        private ThrusterFlameController[] _targetThrusters;

        [Range(0f, 1f)][SerializeField] private float rightRightThruster;
        [Range(0f, 1f)][SerializeField] private float rightTopThruster;
        [Range(0f, 1f)][SerializeField] private float rightBottomThruster;
        [Range(0f, 1f)][SerializeField] private float rightBackThruster;
        [Range(0f, 1f)][SerializeField] private float rightFrontThruster;
        [Range(0f, 1f)][SerializeField] private float leftFrontThruster;
        [Range(0f, 1f)][SerializeField] private float leftBackThruster;
        [Range(0f, 1f)][SerializeField] private float leftBottomThruster;
        [Range(0f, 1f)][SerializeField] private float leftTopThruster;
        [Range(0f, 1f)][SerializeField] private float leftLeftThruster;

        private float[] thrusterValues;

        private void Start()
        {

        }

        private void Update()
        {
            thrusterValues =
            [
                rightRightThruster,
                rightTopThruster,
                rightBottomThruster,
                rightBackThruster,
                rightFrontThruster,
                leftFrontThruster,
                leftBackThruster,
                leftBottomThruster,
                leftTopThruster,
                leftLeftThruster
            ];

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
