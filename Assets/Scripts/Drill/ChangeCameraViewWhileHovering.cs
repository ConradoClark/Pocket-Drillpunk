using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Licht.Unity.Objects;
using UnityEngine;

public class ChangeCameraViewWhileHovering : MonoBehaviour
{
    private CinemachineVirtualCamera _virtualCamera;
    private DrillCharacterController _drillCharacterController;
    private CinemachineFramingTransposer _framingTransposer;
    private float _originalScreenY;
    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _drillCharacterController = SceneObject<DrillCharacterController>.Instance();
        _framingTransposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        _originalScreenY = _framingTransposer.m_ScreenY;
    }

    // Update is called once per frame
    void Update()
    {
        if (_drillCharacterController != null)
        {
            _framingTransposer.m_ScreenY = _drillCharacterController.IsHovering ? 0.6f : _originalScreenY;
        }
    }
}
