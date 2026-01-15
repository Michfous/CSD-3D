using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public static class ConfigService
{
    public static EventConfig Config { get; private set; }
    public static bool IsLoaded { get; private set; }

    public static IEnumerator Initialize()
    {
        yield return LoadConfig();
        IsLoaded = true;
    }

    static IEnumerator LoadConfig()
    {
        string path = System.IO.Path.Combine(
            Application.streamingAssetsPath,
            "game_config.json"
        );

        using (UnityWebRequest request = UnityWebRequest.Get(path))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Config load failed: " + request.error);
                Config = new EventConfig(); // fallback
                yield break;
            }

            Config = JsonUtility.FromJson<EventConfig>(
                request.downloadHandler.text
            );
        }
    }

    public static void Print()
    {
        if (IsLoaded)
        {
            string title = Config.title;
            if (title == null)
            {
                Debug.Log("No title for this event");
            }
            else
            {
                Debug.Log(title);
            }
            string location = Config.location;
            if (location == null)
            {
                Debug.Log("No location for this event");
            }
            else
            {
                Debug.Log(location);
            }
            string posterPath = Config.posterUrl;
            if (posterPath == null)
            {
                Debug.Log("No poster for this event");
            }
            else
            {
                Debug.Log(posterPath);
            }
            string eventWebsite = Config.eventWebsite;
            if (eventWebsite == null)
            {
                Debug.Log("No eventWebsite for this event");
            }
            else
            {
                Debug.Log(eventWebsite);
            }
        }
        else
        {
            Debug.Log("Config hasn't been loaded");
        }
    }
}
