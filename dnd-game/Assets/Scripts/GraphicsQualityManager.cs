using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicsQualityManager : MonoBehaviour
{
    public static GraphicsQualityManager Instance { get; private set; }

    public bool IsHighQuality { get; private set; }

    void Start()
    {

    }

    void Update()
    {

    }

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void SetHighQuality(bool highQuality)
    {
        IsHighQuality = highQuality;
        QualitySettings.SetQualityLevel(highQuality ? QualitySettings.count - 1 : 0);
    }
}
