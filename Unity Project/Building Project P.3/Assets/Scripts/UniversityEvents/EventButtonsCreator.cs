using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static UnityEngine.Rendering.STP;

public class EventButtonsCreator : MonoBehaviour
{
    public GameObject templatePanel;
    public Transform scrollContent;

    IEnumerator Start()
    {
        yield return ConfigService.Initialize();

        yield return PopulateScrollView();
    }

    private IEnumerator LoadImageCoroutine(Image targetImage, string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
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
                new Vector2(0.5f, 0.5f),
                100f
            );

            targetImage.sprite = sprite;
            targetImage.preserveAspect = true;
        }
    }

    private IEnumerator NewEventPanel(EventConfig eventConfig)
    {
        GameObject newPanel = Instantiate(templatePanel, scrollContent);
        newPanel.SetActive(true);

        TMP_Text title = newPanel.transform.Find("Info/Title").GetComponent<TMP_Text>();
        TMP_Text location = newPanel.transform.Find("Info/Navigation Panel/Navigation Location/Text (TMP)").GetComponent<TMP_Text>();
        TMP_Text linkText = newPanel.transform.Find("Info/Link/Text (TMP)").GetComponent<TMP_Text>();
        OpenLink openLinkScript = newPanel.transform.Find("Info/Link").GetComponent<OpenLink>();
        Image poster = newPanel.transform.Find("Poster Frame/Poster").GetComponent<Image>();

        title.text = eventConfig.title;
        location.text = eventConfig.location;
        linkText.text = eventConfig.eventWebsite;
        openLinkScript.url = eventConfig.eventWebsite;

        string path = System.IO.Path.Combine(
            Application.streamingAssetsPath,
            eventConfig.posterUrl
        );
        yield return LoadImageCoroutine(poster, path);
    }

    IEnumerator PopulateScrollView()
    {
        yield return NewEventPanel(ConfigService.Config);

        templatePanel.SetActive(false);
    }
}
