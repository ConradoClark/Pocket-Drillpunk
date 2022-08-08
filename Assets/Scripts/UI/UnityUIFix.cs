using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityUIFix : MonoBehaviour
{
    public Vector3 Offset;
    // Start is called before the first frame update
    void Start()
    {
        if (!Application.isEditor)
        {
            transform.localPosition += Offset;
        }
    }
}
