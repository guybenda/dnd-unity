using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessageScript : MonoBehaviour
{
    const float fadeOutDelay = 8f;
    const float fadeOutTime = 2f;
    const float initialBackgroundAlpha = 0.5f;

    float opacity = 1f;

    TMP_Text Text;
    Image Background;

    void Start()
    {
        StartCoroutine(FadeOut());
    }

    void Update()
    {
        if (!ChatManager.Instance) return;

        if (ChatManager.Instance.IsHistoryVisible)
        {
            Text.color = new(1, 1, 1, 1);
            Background.color = new(0, 0, 0, initialBackgroundAlpha);
        }
        else
        {
            Text.color = new(1, 1, 1, opacity);
            Background.color = new(0, 0, 0, initialBackgroundAlpha * opacity);
        }
    }

    void Awake()
    {
        Text = transform.Find("Text").GetComponent<TMP_Text>();
        Background = GetComponentInChildren<Image>();
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(fadeOutDelay);

        while (opacity > 0)
        {
            opacity = Mathf.MoveTowards(opacity, 0, Time.deltaTime / fadeOutTime);
            yield return null;
        }
    }
}
