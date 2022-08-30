using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public GameObject target;

    void LateUpdate()
    {
        if (target != null)
        {
            transform.LookAt(target.transform);
        }
    }
}
