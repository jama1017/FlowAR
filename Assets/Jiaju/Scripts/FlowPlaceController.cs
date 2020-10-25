using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Portalble;


public class FlowPlaceController : PortalbleGeneralController
{
    public Transform placePrefab;
    public float offset = 0.00002f;

    public override void OnARPlaneHit(PortalbleHitResult hit)
    {
        base.OnARPlaneHit(hit);
        Transform poop = null;

        if (placePrefab != null)
        {
            poop = Instantiate(placePrefab, hit.Pose.position + hit.Pose.rotation * Vector3.up * offset, hit.Pose.rotation);
        }

        Vector3 targetPostition = new Vector3(Camera.main.transform.position.x,
                                    poop.position.y,
                                    Camera.main.transform.position.z);

        poop.transform.LookAt(2 * poop.position - targetPostition);
    }
}

