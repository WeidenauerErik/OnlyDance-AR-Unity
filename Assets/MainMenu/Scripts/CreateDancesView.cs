using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

[Serializable]
public class Dance
{
    public int id;
    public string name;
}

[Serializable]
public class DanceWrapper
{
    public Dance[] dances;
}

public class CreateDancesView : MonoBehaviour
{
    
    private class DevCertificateHandler : CertificateHandler
    {
        protected override bool ValidateCertificate(byte[] certificateData) => true;
    }
    
    public static void SetMyDancesIntoView(VisualElement mainView)
    {
        var myDanceList = new List<Dance>
        {
            new Dance { id = 1, name = "Walzer" }
        };

        mainView.Clear();
        mainView.Add(TabSwitcher.CreateHeading("Meine Tänze"));
        CreateDance(mainView, myDanceList);
    }
    
    public async void SetOnlineDancesIntoView(VisualElement mainView)
    {
        mainView.Clear();
        mainView.Add(TabSwitcher.CreateHeading("Online Tänze"));

        try
        {
            List<Dance> dances = await FetchFiveDances("https://37396.hostserv.eu/getFiveDances");
            CreateDance(mainView, dances);
        }
        catch (Exception e)
        {
            Debug.LogError($"Fehler beim Laden der Tänze: {e.Message}");
        }
    }
    
    private static async Task<List<Dance>> FetchFiveDances(string url)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.certificateHandler = new DevCertificateHandler();

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success) throw new Exception(request.error);

            var json = request.downloadHandler.text;
            var wrappedJson = "{\"dances\":" + json + "}";
            DanceWrapper wrapper = JsonUtility.FromJson<DanceWrapper>(wrappedJson);

            return new List<Dance>(wrapper.dances);
        }
    }
    
    private static void CreateDance(VisualElement mainView, IEnumerable<Dance> danceList)
    {
        foreach (var dance in danceList)
        {
            var container = new VisualElement();
            container.AddToClassList("danceContainer");

            var danceNameLabel = new Label(dance.name);
            danceNameLabel.AddToClassList("danceName");
            container.Add(danceNameLabel);

            var dancePlayBtn = new Button();
            dancePlayBtn.AddToClassList("dancePlayBtn");
            dancePlayBtn.RemoveFromClassList("unity-button");
            dancePlayBtn.clicked += () =>
            {
                DanceLoader.Instance.SetDanceCredentials(dance.name, dance.id);
                SceneManager.LoadScene("DanceAnimator");
            };

            container.Add(dancePlayBtn);
            mainView.Add(container);
        }
    }
}
