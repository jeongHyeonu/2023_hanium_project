using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VR_CustomInteract : XRGrabInteractable
{
    [SerializeField] GameObject lifeJacket;

    protected override void OnSelectExited(XRBaseInteractor interactor)
    {
        // 물체를 놓았을 때 좌표 출력
        Vector3 releasePosition = transform.position;
        Debug.Log("Released at position: " + releasePosition);
    }
}
