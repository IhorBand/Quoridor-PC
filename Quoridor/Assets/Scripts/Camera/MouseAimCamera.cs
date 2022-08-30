using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAimCamera : MonoBehaviour
{
    public Transform target = null;
    public float rotateSpeed = 5;
    Vector3 offset;

    public void SetTarget(Transform target, Vector3? offset = null)
    {
        this.target = target;
        transform.rotation = Quaternion.Euler(11, 90, 0);

        if (offset != null)
        {
            this.offset = offset.Value;
        }
        else
        {
            offset = target.transform.position - transform.position;
        }
    }

    void Start()
    {
    }

    void LateUpdate()
    {
        if (target != null)
        {
            float horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
            target.transform.Rotate(0, horizontal, 0);

            float desiredAngle = target.transform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);
            transform.position = target.transform.position - (rotation * offset);

            transform.LookAt(target.transform);
        }
    }
}
