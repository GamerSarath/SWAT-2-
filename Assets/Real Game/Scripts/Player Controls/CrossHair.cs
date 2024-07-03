using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHair : MonoBehaviour
{
    [SerializeField] Transform gunPoint;
    RaycastHit hitinfo;
    // Update is called once per frame
    void FixedUpdate()
    {
        Ray rayOrigin  = Camera.main.ScreenPointToRay( Input.mousePosition);
        if(Physics.Raycast(rayOrigin, out hitinfo))
        {
            if(hitinfo.collider != null)
            {
                gunPoint.rotation = Quaternion.LookRotation(hitinfo.point);     
            }
        }
    }
}
