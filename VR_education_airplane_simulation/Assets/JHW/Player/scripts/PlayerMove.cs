using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] Vector3 offsetVector;
    [SerializeField] float playerSpeed;
    [SerializeField] float rotateSpeed = 5;
    [SerializeField] GameObject playerXROrigin;

    Animator ani;
    bool isKeyDown;
    Vector3 dir;

    private void Start()
    {
    }

    private void Update()
    {
        // 방향키
        //float hAxis = Input.GetAxisRaw("Horizontal");
        //float vAxis = Input.GetAxisRaw("Vertical");
        //if (hAxis == 0 && vAxis == 0) { ani.SetBool("walk",false); isKeyDown = false; }
        //else { isKeyDown = true; ani.SetBool("walk", true); dir = new Vector3(hAxis, 0, vAxis); }

        float X_Move = Input.GetAxisRaw("Oculus_GearVR_RThumbstickX");
        float Y_Move = Input.GetAxisRaw("Oculus_GearVR_RThumbstickY");
        Debug.Log(X_Move + ", " + Y_Move);
        if(X_Move==0 && Y_Move==0) {  isKeyDown = false; }
        else { dir = new Vector3 (X_Move,0, Y_Move); isKeyDown = true; }

        // 이동 및 회전
        if (isKeyDown)
        {
            playerXROrigin.transform.position += playerSpeed * Time.deltaTime * dir;
            playerXROrigin.transform.rotation = Quaternion.Lerp(playerXROrigin.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotateSpeed);

            //playerXROrigin.transform.position += dir * playerSpeed * Time.deltaTime;
            //playerXROrigin.transform.rotation = Quaternion.Lerp(playerXROrigin.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotateSpeed);
            // 카메라
            //cam.transform.position = this.transform.position + offsetVector;
            //CamOffset.transform.position = this.transform.position + offsetVector;
        }


    }
}
