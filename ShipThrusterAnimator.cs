using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace ExpeditionTakeoff
{
    [ExecuteInEditMode]
    public class ShipThrusterAnimator : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] _targetThrusters;

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

        [SerializeField] private bool syncYThrusters;
        [SerializeField] private bool syncZThrusters;

        private float _thrusterLightIntensity = 0.3f;
        private float _thrusterLightRange = 150f;
        private float[] _thrusterValues;
        private List<Light> _thrusterLights = [];

        private void Start()
        {
            for (int i = 0; i < _targetThrusters.Length; i++)
            {
                _thrusterLights.Add(_targetThrusters[i].GetComponentInChildren<Light>());
                _thrusterLights[i].intensity = _thrusterLightIntensity;
                _thrusterLights[i].range = _thrusterLightRange;
            }
        }

        private void Update()
        {
            _thrusterValues =
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

            if (_targetThrusters.Length == 0 || _thrusterValues.Length != _targetThrusters.Length)
            {
                Debug.LogError("Check the thruster/value lists, something is wrong with them.");
                return;
            }
            if (syncYThrusters)
            {
                leftBottomThruster = rightBottomThruster;
                leftTopThruster = rightTopThruster;
            }
            if (syncZThrusters)
            {
                leftFrontThruster = rightFrontThruster;
                leftBackThruster = rightBackThruster;
            }
            for (int i = 0; i < _targetThrusters.Length; i++)
            {
                _targetThrusters[i].transform.localScale = Vector3.one * _thrusterValues[i];
                if (_thrusterValues[i] != 0f)
                {
                    _thrusterLights[i].enabled = true;
                    _thrusterLights[i].range = _thrusterLightRange * _thrusterValues[i];
                    _targetThrusters[i].GetComponent<MeshRenderer>().enabled = true;
                }
                else
                {
                    _thrusterLights[i].enabled = false;
                    _targetThrusters[i].GetComponent<MeshRenderer>().enabled = false;
                }
            }
        }
    }
}
