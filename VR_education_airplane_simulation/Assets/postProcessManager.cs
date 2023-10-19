using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class postProcessManager : MonoBehaviour
{
    private static postProcessManager instance = null;

    bool isLightOn = false;
    float duration = 2f;

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    public static postProcessManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    [SerializeField] private Volume postProcessingVolume;
    [SerializeField] private ColorAdjustments colorAdjustments;

    // Start is called before the first frame update
    void Start()
    {
        postProcessingVolume = this.GetComponent<Volume>();
        postProcessingVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
    }

    public void RedLightOn()
    {
        isLightOn = true;
        colorAdjustments.active = true;
        StartCoroutine(AutoLight());
    }

    public void RedLightOff()
    {
        isLightOn = false;
        colorAdjustments.active = false;
        StopCoroutine(AutoLight());
    }

    IEnumerator AutoLight(bool isOnOff = true)
    {
        if (isLightOn)
        {
            colorAdjustments.active = isOnOff;

            if (isOnOff) yield return new WaitForSeconds(duration - 1);
            else yield return new WaitForSeconds(duration);

            StartCoroutine(AutoLight(!isOnOff));
        }
        //if (ascent)
        //{
        //    while (progress < duration)
        //    {
        //        progress += Time.deltaTime;
        //        float r = 1f;
        //        float g = 1f - progress/duration;
        //        float b = 1f - progress/duration;
        //        colorAdjustments.colorFilter = new ColorParameter(new Color(r, g, b),true);
        //        Debug.Log(progress);
        //    }
        //}
        //else
        //{
        //    while (progress < duration)
        //    {
        //        progress += Time.deltaTime;
        //        float r = 1f;
        //        float g = progress / duration;
        //        float b = progress / duration;
        //        colorAdjustments.colorFilter = new ColorParameter(new Color(r, g, b),true);
        //    }
        //}
        //yield return new WaitForSeconds(Time.deltaTime);
        //StartCoroutine(AutoLight(!ascent,progress%5));
    }

    //private void FixedUpdate()
    //{
    //    if (isRedLight == false) return;

    //    progress += Time.deltaTime;

    //    if (ascent)
    //    {
    //        float r = 1f;
    //        float g = 1f - progress / duration;
    //        float b = 1f - progress / duration;
    //        colorAdjustments.colorFilter = new ColorParameter(new Color(r, g, b), true);
    //    }
    //    else
    //    {
    //        float r = 1f;
    //        float g = progress / duration;
    //        float b = progress / duration;
    //        colorAdjustments.colorFilter = new ColorParameter(new Color(r, g, b), true);
    //    }

    //    if (progress > duration) { progress = 0f; ascent = !ascent; }
    //}
}
