using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

namespace DndFirebase
{
    public class AuthManager : MonoBehaviour
    {
        public static AuthManager Instance { get; private set; }
        public User CurrentUser { get; private set; }

        event Action<User> OnUserLoaded;

        FirebaseAuth auth;

        readonly object _userLock = new();

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

            if (auth.CurrentUser != null)
            {
                User.Get(auth.CurrentUser.Email).ContinueWith(task =>
                {
                    if (!task.IsCompletedSuccessfully)
                    {
                        Debug.LogError("Failed to get user: " + task.Exception);
                        return;
                    }

                    lock (_userLock)
                    {
                        CurrentUser = task.Result;
                        OnUserLoaded?.Invoke(CurrentUser);
                    }
                });
            }
        }

        public async Task<(bool success, string error)> SignUp(string displayName, string email, string password)
        {
            try
            {
                AuthResult result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
                FirebaseUser fbUser = result.User;

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

        public void AddOnUserLoadedListener(Action<User> listener)
        {
            lock (_userLock)
            {
                if (CurrentUser == null)
                {
                    OnUserLoaded += listener;
                    return;
                }
            }

            listener(CurrentUser);
        }

        public void RemoveOnUserLoadedListener(Action<User> listener)
        {
            OnUserLoaded -= listener;
        }
    }
}