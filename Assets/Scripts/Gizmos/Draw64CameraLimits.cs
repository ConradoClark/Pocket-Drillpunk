using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Draw64CameraLimits : MonoBehaviour
{
    public PixelPerfectCamera Camera;
    // Start is called before the first frame update
    void Start()
    {

    }

    void OnDrawGizmos()
    {
        if (Camera != null)
        {
            // Draws a blue line from this transform to the target
            Gizmos.color = Color.yellow;

            var pos = new Vector3(Camera.transform.position.x, Camera.transform.position.y,0);
            var size = Camera.orthographicSize;

            Gizmos.DrawLine(pos - new Vector3(size, size), pos + new Vector3(size, -size));
            Gizmos.DrawLine(pos - new Vector3(size, -size), pos + new Vector3(size, size));
            Gizmos.DrawLine(pos - new Vector3(size, size), pos - new Vector3(size, -size));
            Gizmos.DrawLine(pos + new Vector3(size, -size), pos + new Vector3(size, size));
        }
    }
}
