using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class InternetChecker : MonoBehaviour
{
    private string testUrl;
    private float checkInterval = 5f;

    private void Start()
    {
        testUrl = PlayerPrefs.GetString("url");
        PopUpManagerGeneral.Initialize();
        StartCoroutine(CheckInternetLoop());
    }

    private IEnumerator CheckInternetLoop()
    {
        while (true)
        {
            yield return StartCoroutine(CheckInternet());
            yield return new WaitForSeconds(checkInterval);
        }
    }

    private IEnumerator CheckInternet()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(testUrl))
        {
            request.timeout = 5;
            yield return request.SendWebRequest();
            
            if (request.result != UnityWebRequest.Result.ConnectionError && !string.IsNullOrEmpty(request.downloadHandler.text)) Debug.Log($"✅ Internetverbindung aktiv ({testUrl} antwortete)");
            else
            {
                Debug.Log($"❌ Keine Verbindung oder keine Antwort von {testUrl}");
                PopUpManagerGeneral.ShowInfo("Internet", "Du hast leider keine Internetverbindung!");
            }
        }
    }
}