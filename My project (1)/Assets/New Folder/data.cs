using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class data : MonoBehaviour
{
    public TMPro.TMP_InputField name_field;
    public TMPro.TMP_InputField stnum_field;
    public TMPro.TMP_InputField contacts_field;

    public string name;
    public int stnum=0;
    public string contacts;
    public int attempts;
    public bool cleared;
    public string record;


    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void saveData()
    {
        name = name_field.text;
        contacts = contacts_field.text;
        stnum = int.Parse(stnum_field.text);

        if (name == "" || stnum == 0 || contacts == "")
        {
            Debug.Log("Please fill in all fields.");
            return;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("GAMEPLAYING");
    }

    public void CheckIsUserPlayed()
    {
        // api 스크립트 참조
        api myApi = FindObjectOfType<api>();
        stnum = int.Parse(stnum_field.text);

        // 검사 요청
        myApi.CheckPlayerStatus(stnum, (bool isPlayed) => {
            // 이 중괄호 안의 코드는 서버 응답이 온 직후 실행됩니다.
            if (isPlayed)
            {
                Debug.Log("이미 플레이한 유저입니다.");
                // 팝업을 띄우거나 입장을 막는 코드
            }
            else
            {
                Debug.Log("처음 하는 유저입니다.");
                // 게임 시작 코드
                saveData();
            }
        });
    }
}
