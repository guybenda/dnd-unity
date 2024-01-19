using DndFirebase;
using UnityEngine;

public class EverythingManager : MonoBehaviour
{
    static EverythingManager Instance;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("Multiple EverythingManagers, destroying");
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);

        gameObject.AddComponent<DiceManager>();
        gameObject.AddComponent<AuthManager>();
        gameObject.AddComponent<DndFirestore>();
        gameObject.AddComponent<SceneTransitionManager>();
        gameObject.AddComponent<DiceMaterialManager>();
    }
}