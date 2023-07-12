using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class CustomGrabInteract : XRGrabInteractable
{
    [SerializeField] Transform left_grab_pos;
    [SerializeField] Transform right_grab_pos;

    protected override void OnSelectEntered(XRBaseInteractor interactor)
    {
        if(interactor.name=="LeftHand Controller")
        {
            this.attachTransform = left_grab_pos;
            this.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        }
        else if (interactor.name == "RightHand Controller")
        {
            this.attachTransform = right_grab_pos;
            this.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        }
    }
}
