using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Firestore;
using UnityEngine;

namespace DndFirebase
{
    public class AuthManager : MonoBehaviour
    {
        public static AuthManager Instance { get; private set; }
        public User CurrentUser { get; private set; }

        FirebaseAuth auth;

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

        public async Task<(bool success, string error)> SignUp(string displayName, string email, string password)
        {
            try
            {
                AuthResult result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
                FirebaseUser fbUser = result.User;

                await fbUser.UpdateUserProfileAsync(new()
                {
                    DisplayName = displayName
                });

                var user = new User
                {
                    DisplayName = displayName,
                    Email = email,
                    Dice = UserDice.Default()
                };

                await user.Save();

                CurrentUser = user;

            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return (false, e.Message);
            }

            return (true, null);
        }

        public async Task<(bool success, string error)> Login(string email, string password)
        {
            try
            {
                var result = await auth.SignInWithEmailAndPasswordAsync(email, password);
                var user = await User.Get(result.User.Email);
                user.DisplayName = result.User.DisplayName;

                CurrentUser = user;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return (false, e.Message);
            }

            return (true, null);
        }

        public bool Logout()
        {
            try
            {
                auth.SignOut();
                CurrentUser = null;
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