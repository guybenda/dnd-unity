using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Firestore;
using UnityEngine;

public class DndFirebaseDb : MonoBehaviour
{
    public static DndFirebaseDb Instance { get; private set; }

    FirebaseFirestore db;

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

        db = FirebaseFirestore.DefaultInstance;
    }


}
