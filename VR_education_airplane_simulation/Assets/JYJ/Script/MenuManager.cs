using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

// 싱글톤 및 변수 선언
partial class MenuManager : MonoBehaviour
{
    private static MenuManager instance = null;

    void Awake()
    {
        if (null == instance)
        {
            //이 클래스 인스턴스가 탄생했을 때 전역변수 instance에 인스턴스가 담겨있지 않다면, 자신을 넣어준다.
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            //만약 씬 이동이 되었는데 그 씬에도 Hierarchy에 오브젝트 존재할 수 있으므로
            Destroy(this.gameObject);
        }
    }

    //게임 매니저 인스턴스에 접근할 수 있는 프로퍼티. static이므로 다른 클래스에서 맘껏 호출할 수 있다.
    public static MenuManager Instance
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

    private void Start()
    {
        
    }
}

// 메뉴화면 버튼
partial class MenuManager : MonoBehaviour
{
    public void MenuButton_OnMouseClick(GameObject _target)
    {
        // 버튼 클릭시 실행할 함수
    }
}