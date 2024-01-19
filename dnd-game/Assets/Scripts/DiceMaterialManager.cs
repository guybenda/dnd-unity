using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceMaterialManager : MonoBehaviour
{
    public static DiceMaterialManager Instance { get; private set; }

    readonly Dictionary<UserDice, DiceMaterial> diceMaterials = new();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this);
    }
}
