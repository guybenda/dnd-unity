using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Firestore;
using UnityEngine;

namespace DndFirebase
{

    public class DndFirestore : MonoBehaviour
    {
        public static DndFirestore Instance { get; private set; }

        public FirebaseFirestore Db;


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

            Db = FirebaseFirestore.DefaultInstance;
        }


    }
}
