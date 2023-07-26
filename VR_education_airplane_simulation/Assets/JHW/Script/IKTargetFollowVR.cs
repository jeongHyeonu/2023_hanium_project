using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VRMap
{
    public Transform vrTarget;
    public Transform ikTarget;
    public Vector3 trackingPosOffset;
    public Vector3 trackingRotOffset;

    public void Map()
    {
        ikTarget.position = vrTarget.TransformPoint(trackingPosOffset);
        ikTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotOffset);
    }
}


public class IKTargetFollowVR : MonoBehaviour
{
    [Range(0, 1)]
    public float turnSmoothness = .1f;
    public VRMap head;
    public VRMap leftHand;
    public VRMap rightHand;

    public Vector3 headBodyPosOffset;
    public float headBodyYawOffset;

    private void LateUpdate()
    {
        transform.position = head.ikTarget.position + headBodyPosOffset;
        float yaw = head.vrTarget.eulerAngles.y;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, yaw,transform.eulerAngles.z),turnSmoothness);

        head.Map();
        leftHand.Map();
        rightHand.Map();
    }
}
