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

    public Playermove player;      // 플레이어 오브젝트

    public TextMeshProUGUI timeText; // UI에 연결할 TMP 텍스트
    public TextMeshProUGUI distanceText; // UI에 연결할 TMP 텍스트

    public float times = 0;                // 현재 시간

    private data data; // data ����� ����

    void Start()
    {
        if (isCountDown)
        {
            displayTime = gameTime;
        }

        data = FindObjectOfType<data>();
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
                    data.attempts = player.attempts;
                    data.cleared = false;
                    data.record = "99:99:999";
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
        distanceText.text = data.distance + "m";
    }

    public string GetTimeString(int maxTime)
    {
        int intTime = maxTime - Mathf.CeilToInt(displayTime);
        return intTime.ToString();
    }

    public string FormatTime(float timeToFormat)
    {
        // 시간을 분, 초, 밀리초로 분리
        
        // 1. 전체 시간(초)을 정수로 변환하여 분을 계산합니다.
        // (float)times / 60f의 정수 부분이 '분'이 됩니다.
        int minutes = (int)(timeToFormat / 60f);

        // 2. 전체 시간(초)에서 분에 해당하는 초를 뺀 나머지로 초를 계산합니다.
        // timeToFormat % 60f는 60초마다 0으로 리셋되는 '초'와 '밀리초'를 포함합니다.
        // 이 값의 정수 부분이 '초'가 됩니다.
        int seconds = (int)(timeToFormat % 60f);

        // 3. 밀리초를 계산합니다.
        // timeToFormat * 1000f를 하면 전체 시간이 밀리초 단위가 됩니다.
        // 이 값에서 (분 * 60 * 1000) + (초 * 1000)을 뺀 나머지가 밀리초가 됩니다.
        // 또는, (timeToFormat % 1f) * 1000f를 사용하여 소수점 이하 부분만 밀리초로 변환합니다.
        int milliseconds = (int)((timeToFormat * 1000f) % 1000f);
        
        // 4. 문자열 포맷팅
        // String.Format을 사용하여 각 숫자를 지정된 자릿수로 포맷합니다.
        // {0:00} -> 2자릿수, 1자릿수일 경우 앞에 0 추가 (예: 9 -> 09)
        // {2:000} -> 3자릿수 (예: 12 -> 012)
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
}


