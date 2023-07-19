using Amazon.Runtime.Internal.Transform;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class SoundManager : MonoBehaviour
{
    private static SoundManager instance; // 싱글톤

    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public static SoundManager Instance
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
}
public partial class SoundManager : MonoBehaviour { 
    [SerializeField] Dictionary<BGM_list, AudioClip> BGM_audioclips = new Dictionary<BGM_list, AudioClip>();
    [SerializeField] Dictionary<SFX_list, AudioClip> SFX_audioclips = new Dictionary<SFX_list, AudioClip>();

    [SerializeField] public List<BGM_Datas> BGM_datas = new List<BGM_Datas>();
    [SerializeField] public List<SFX_Datas> SFX_datas = new List<SFX_Datas>();

    [SerializeField] GameObject BGM_speaker;
    [SerializeField] GameObject SFX_speaker;

    [SerializeField] float BGM_volumeScale;
    [SerializeField] float SFX_volumeScale;

    [System.Serializable]
    [SerializeField]
    public struct SFX_Datas
    {
        public SFX_list sfx_name;
        public AudioClip audio;
    }

    [System.Serializable]
    [SerializeField]
    public struct BGM_Datas
    {
        public BGM_list bgm_name;
        public AudioClip audio;
    }

    // 배경음 목록
    public enum BGM_list
    {
        temp_BGM,
    }


    // 효과음 목록
    public enum SFX_list
    {
        button1,
        button2,
        button3,
        button4,
        button5,
    }

    private void Start()
    {
        // 사운드 딕셔너리에 저장
        for (int i = 0; i < BGM_datas.Count; i++) {
            BGM_audioclips.Add(BGM_datas[i].bgm_name, BGM_datas[i].audio);
        }

        for (int i = 0; i < SFX_datas.Count; i++)
        {
            SFX_audioclips.Add(SFX_datas[i].sfx_name, SFX_datas[i].audio);
        }
    }

    // 배경음 조정
    public void volumeScale_BGM_OnChange(GameObject slider)
    {
        BGM_volumeScale = slider.GetComponent<Slider>().value;
        BGM_speaker.GetComponent<AudioSource>().volume = BGM_volumeScale;
    }

    // 효과음 조정
    public void volumeScale_SFX_OnChange(GameObject slider)
    {
        SFX_volumeScale = slider.GetComponent<Slider>().value;
        SFX_speaker.GetComponent<AudioSource>().volume = SFX_volumeScale;
        TextToSpeach.Instance.GetComponent<AudioSource>().volume = SFX_volumeScale;
    }


    // 배경음 재생
    public void PlayBGM(BGM_list bgm)
    {
        BGM_speaker.GetComponent<AudioSource>().clip = BGM_audioclips[bgm];
        BGM_speaker.GetComponent<AudioSource>().Play();
    }

    // 효과음 재생
    public void PlaySFX(SFX_list sfx)
    {
        SFX_speaker.GetComponent<AudioSource>().PlayOneShot(SFX_audioclips[sfx], SFX_volumeScale);
    }
}
