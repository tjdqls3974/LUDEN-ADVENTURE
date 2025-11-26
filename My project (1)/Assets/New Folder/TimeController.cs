using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TMP 사용

public class TimeController : MonoBehaviour
{
    public bool isCountDown = true; // true= 카운트 다운으로 시간 측정
    public float gameTime = 0;      // 게임의 최대 시간
    public bool isTimeOver = false; // true= 타이머 정지
    public float displayTime = 0;   // 표시 시간

    public TextMeshProUGUI timeText; // UI에 연결할 TMP 텍스트

    float times = 0;                // 현재 시간

    void Start()
    {
        if (isCountDown)
        {
            displayTime = gameTime;
        }
        UpdateTimeText(); // 처음부터 표시
    }

    void Update()
    {
        if (!isTimeOver)
        {
            times += Time.deltaTime;
            if (isCountDown)
            {
                displayTime = gameTime - times;
                if (displayTime <= 0.0000f)
                {
                    displayTime = 0.0000f;
                    isTimeOver = true;

                    // 씬 이동
                    SceneManager.LoadScene("end");
                }
            }
            else
            {
                displayTime = times;
                if (displayTime >= gameTime)
                {
                    displayTime = gameTime;
                    isTimeOver = true;
                }
            }

            UpdateTimeText();
        }
    }

    // TMP 텍스트 업데이트
    void UpdateTimeText()
    {
        int intTime = Mathf.CeilToInt(displayTime); // 남은 시간 정수화 (10.9초 → 11)
        timeText.text = intTime.ToString();
    }
}


