using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;

namespace DndFirebase
{
    public class DndFirebaseAuth : MonoBehaviour
    {
        public static DndFirebaseAuth Instance { get; private set; }

        FirebaseAuth auth;

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

            auth = FirebaseAuth.DefaultInstance;
        }

        public async Task<bool> SignUp(string displayName, string email, string password)
        {
            try
            {
                AuthResult result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
                FirebaseUser user = result.User;

                await user.UpdateUserProfileAsync(new()
                {
                    DisplayName = displayName
                });
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            return true;
        }

        public async Task<bool> Login(string email, string password)
        {
            try
            {
                var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            return true;
        }

        public bool Logout()
        {
            try
            {
                auth.SignOut();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }

            return true;
        }

        public bool IsLoggedIn()
        {
            return auth.CurrentUser != null;
        }
    }
}