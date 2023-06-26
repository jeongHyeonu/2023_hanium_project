using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class buttonUX : MonoBehaviour
{
    [SerializeField] float size = 1.3f;
    [SerializeField] float speed = .5f;

    // Start is called before the first frame update
    void Start()
    {
        this.transform.DOScale(size, speed).SetLoops(-1,LoopType.Yoyo);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
