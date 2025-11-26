using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeController : MonoBehaviour{
    public bool isCountDown = true; // true= 카운트 다운으로 시간 측정
    public float gameTime = 0;      // 게엠의 최대 시간
    public bool isTimeOver = false; // true= 타이머 정지
    public float displayTime = 0;   // 표시 시간

    float times = 0;                // 현재 시간

    // Start is called before the first frame update
    void Start(){
        if (isCountDown){
            // 카운트다운
            displayTime = gameTime;
        }
    }

    // Update is called once per frame
    void Update(){
        if (isTimeOver == false){
            times += Time.deltaTime;
            if (isCountDown){
                // 카운트다은
                displayTime = gameTime - times;
                if (displayTime <= 0.0f){
                    displayTime = 0.0f;
                    isTimeOver = true;

                    // ★ 여기서 씬 이동 ★
                    SceneManager.LoadScene("end");
                }
            }else{
                // 카운트업
                displayTime = times;
                if (displayTime >= gameTime){
                    displayTime = gameTime;
                    isTimeOver = true;
                }
            }
            Debug.Log("TIMES: " + displayTime);
        }
    }
}

