using UnityEngine;
using Cinemachine;

public class CinemachineInfiniteBoundary : CinemachineExtension
{
    [SerializeField] Transform leftAnchor;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body && leftAnchor != null)
        {
            float orthographicSize = state.Lens.OrthographicSize;
            float aspectRatio = state.Lens.Aspect;
            float halfWidth = orthographicSize * aspectRatio;

            Vector3 pos = state.RawPosition;

            float minX = leftAnchor.position.x + halfWidth;
            
            if (pos.x < minX)
            {
                pos.x = minX;
            }

            state.RawPosition = pos;
        }
    }
}