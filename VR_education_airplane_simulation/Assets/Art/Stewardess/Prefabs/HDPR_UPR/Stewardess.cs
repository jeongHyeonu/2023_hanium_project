using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stewardess : MonoBehaviour
{
    public void TalkStop()
    {
        this.GetComponent<Animator>().SetBool("Talk", false);
    }
}
