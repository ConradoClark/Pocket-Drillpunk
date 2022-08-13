using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-99999)]
public class UnityUIFix : MonoBehaviour
{
    public Vector3 Offset;
    // Start is called before the first frame update
    private void Awake()
    {
        if (!Application.isEditor)
        {
            transform.localPosition += Offset;
        }
    }
}
