using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    float min = 0f;
    float max = 20f;

    // Update is called once per frame
    void Update()
    {
        if (target == null)
            return;

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(target.position.x, min, max);

        transform.position = pos;
    }
}
