using System.Collections;
using System.Collections.Generic;
using System.Text; // Encoding을 위해 필요
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement; // 씬 전환을 위해 필수

public class api : MonoBehaviour
{
    public string apiURL = "http://2-6.site:50208/api/";
    
    // 인스펙터에서 이동할 씬 이름을 적어주세요
    public string nextSceneName = "start"; 

    private data data;

    // Start is called before the first frame update
    void Start()
    {
        data = FindObjectOfType<data>();
    }

    public void OnButtonClick()
    {
        // 데이터 전송 시작
        StartCoroutine(SendPostDataRequest(data.name, data.stnum, data.contacts, data.attempts, data.cleared, data.record, data.distance));
    }

    // ---------------------------------------------------------
    // 1. 데이터 전송 및 성공 시 씬 전환 기능
    // ---------------------------------------------------------
    private IEnumerator SendPostDataRequest(string name, int stnum, string contacts, int attempts, bool clear, string record, string distance)
    {
        // JSON 생성
        string jsonPayload = "{\"name\": \"" + name + "\", \"stnum\": \"" + stnum + "\", \"contacts\": \"" + contacts + "\", \"attempts\": " + attempts + ", \"cleared\": " + clear.ToString().ToLower() + ", \"record\": \"" + record + "\", \"distance\": \"" + distance + "\"}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

        UnityWebRequest request = new UnityWebRequest(apiURL + "csv/", "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            Debug.Log("Success! Response: " + request.downloadHandler.text);
            
            // [수정됨] 요청이 성공했으므로 씬을 넘깁니다.
            SceneManager.LoadScene(nextSceneName);
            Destroy(data.gameObject);
        }
    }

    // ---------------------------------------------------------
    // 2. Check 기능 (stnum을 보내고 bool값을 받아옴)
    // ---------------------------------------------------------
    
    // 이 함수를 외부에서 호출하여 검사를 시작합니다.
    // 사용법: CheckPlayerStatus(학번, (결과) => { 결과처리코드 });
    public void CheckPlayerStatus(int stnum, System.Action<bool> onResult)
    {
        StartCoroutine(SendCheckRequest(stnum, onResult));
    }

    private IEnumerator SendCheckRequest(int stnum, System.Action<bool> onResult)
    {
        // stnum을 JSON으로 만듦 (기존 코드 스타일에 맞춤)
        string jsonPayload = "{\"stnum\": \"" + stnum + "\"}";
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

        // URL 설정 (apiURL + "check/")
        UnityWebRequest request = new UnityWebRequest(apiURL + "check", "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 서버 응답 대기
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("Check Error: " + request.error);
            // 에러 시 false 혹은 에러 처리를 위해 콜백 호출
            onResult(false); 
        }
        else
        {
            Debug.Log("Check Success: " + request.downloadHandler.text);
            
            // JSON 파싱 (서버가 {"isPlayed": true} 같은 형태로 준다고 가정)
            CheckResponse res = JsonUtility.FromJson<CheckResponse>(request.downloadHandler.text);
            
            // 결과 값을 콜백으로 돌려줌
            onResult(res.isPlayed);
        }
    }

    // ---------------------------------------------------------
    // 응답 데이터 클래스 정의
    // ---------------------------------------------------------

    // 기존 ProcessServerResponse에서 사용하던 클래스 (추정)
    [System.Serializable]
    public class FastAPIResponse
    {
        public string message;
    }

    // Check 요청에 대한 응답 클래스
    // 서버에서 보내주는 JSON 키값과 변수명이 정확히 일치해야 합니다.
    [System.Serializable]
    public class CheckResponse
    {
        public bool isPlayed; // 예: {"isPlayed": true}
    }
}