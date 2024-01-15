using DndFirebase;
using UnityEngine;

public class EverythingManager : MonoBehaviour
{
    private static EverythingManager Instance;
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

        // All of these should be relatively stateless
        gameObject.AddComponent<DiceManager>();
        gameObject.AddComponent<DndFirebaseAuth>();
        gameObject.AddComponent<DndFirebaseDb>();
        gameObject.AddComponent<SceneTransitionManager>();
    }
}