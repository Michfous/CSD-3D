using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class EventPanelsCreator : MonoBehaviour
{
    public GameObject templatePanel;
    public Transform scrollContent;
    public float panelScreenHeightPrecentage = 0.6f;

    IEnumerator Start()
    {
        yield return EventsConfigService.Initialize("EventsConfiguration.json");

        yield return PopulateScrollView(EventsConfigService.Config.events);
    }
    
    IEnumerator PopulateScrollView(EventConfig[] events)
    {
        templatePanel.SetActive(false);
        LayoutElement le;

        foreach (var e in events)
        {
            GameObject newPanel = Instantiate(templatePanel, scrollContent);
            le = newPanel.GetComponent<LayoutElement>();
            le.preferredHeight = Screen.height * panelScreenHeightPrecentage;

            yield return PopulateEventPanel(newPanel, e);
            newPanel.SetActive(true);
        }
    }
    
    private IEnumerator PopulateEventPanel(GameObject panel, EventConfig eventConfig)
    {
        PopulateInfoPanel(panel, eventConfig);

        Image poster = panel.transform.Find("Poster Frame/Poster").GetComponent<Image>();

        if (eventConfig.posterUrl != null)
        {
            string path = System.IO.Path.Combine(
                Application.streamingAssetsPath,
                eventConfig.posterUrl
            );
            yield return LoadImageCoroutine(poster, path);
        }
    }

    private void PopulateInfoPanel(GameObject panel, EventConfig eventConfig)
    {
        TMP_Text title = panel.transform.Find("Info Panel/Title").GetComponent<TMP_Text>();
        TMP_Text location = panel.transform.Find("Info Panel/Navigation/Location").GetComponent<TMP_Text>();
        TMP_Text linkText = panel.transform.Find("Info Panel/Link/Text (TMP)").GetComponent<TMP_Text>();
        OpenLink openLinkScript = panel.transform.Find("Info Panel/Link").GetComponent<OpenLink>();

        title.text = eventConfig.title;
        location.text = eventConfig.location;
        linkText.text = eventConfig.eventWebsite;
        openLinkScript.url = eventConfig.eventWebsite;
    }
    
    private IEnumerator LoadImageCoroutine(Image targetImage, string url)
    {
        using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
#if UNITY_WEBGL
        request.SetRequestHeader("Accept", "image/*");
#endif
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Failed to load image: {request.error}");
            yield break;
        }

        Texture2D texture = DownloadHandlerTexture.GetContent(request);

        if (texture == null)
        {
            Debug.LogError("Downloaded texture is null.");
            yield break;
        }

        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f)
        );

        targetImage.sprite = sprite;
        targetImage.preserveAspect = true;
    }
}
