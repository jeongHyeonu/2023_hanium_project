using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] Vector3 offsetVector;
    [SerializeField] float playerSpeed;
    [SerializeField] float rotateSpeed = 5;
    [SerializeField] GameObject CamOffset;

    Animator ani;
    bool isKeyDown;
    Vector3 dir;
    

    private void Start()
    {
        ani = GetComponent<Animator>();
        isKeyDown = false;
        // 카메라
        //cam.transform.position = this.transform.position + offsetVector;
        CamOffset.transform.position = this.transform.position + offsetVector;
    }

    private void Update()
    {
        // 방향키
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");
        if (hAxis == 0 && vAxis == 0) { ani.SetBool("walk",false); isKeyDown = false; }
        else { isKeyDown = true; ani.SetBool("walk", true); dir = new Vector3(hAxis, 0, vAxis); }

        // 이동 및 회전
        if (isKeyDown)
        {
            transform.position += dir * playerSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * rotateSpeed);
            // 카메라
            //cam.transform.position = this.transform.position + offsetVector;
            CamOffset.transform.position = this.transform.position + offsetVector;
        }


    }
}
