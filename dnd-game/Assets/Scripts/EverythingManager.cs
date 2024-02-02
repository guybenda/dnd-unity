using DndFirebase;
using UnityEngine;

public class EverythingManager : MonoBehaviour
{
    static EverythingManager Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);

        gameObject.AddComponent<CrashlyticsInit>();
        gameObject.AddComponent<DiceManager>();
        gameObject.AddComponent<DndFirestore>();
        gameObject.AddComponent<AuthManager>();
        gameObject.AddComponent<SceneTransitionManager>();
        gameObject.AddComponent<DiceMaterialManager>();
    }
}