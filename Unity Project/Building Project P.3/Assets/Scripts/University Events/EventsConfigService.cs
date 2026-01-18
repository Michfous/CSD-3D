using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public static class EventsConfigService
{
    public static EventConfigList Config { get; private set; }
    public static bool IsLoaded { get; private set; }

    public static IEnumerator Initialize(string fileName)
    {
        IsLoaded = false;
        string path = System.IO.Path.Combine(
            Application.streamingAssetsPath,
            fileName
        );
        yield return LoadConfig(path);
    }

    private static IEnumerator LoadConfig(string url)
    {
        using UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Config load failed: " + request.error);
            IsLoaded = false;
            yield break;
        }

        Config = JsonUtility.FromJson<EventConfigList>(
            request.downloadHandler.text
        );
        IsLoaded = true;
    }
}