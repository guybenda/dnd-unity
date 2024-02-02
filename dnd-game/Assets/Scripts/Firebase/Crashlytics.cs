using Firebase.Crashlytics;
using UnityEngine;

public class CrashlyticsInit : MonoBehaviour
{
    void Awake()
    {
        Crashlytics.ReportUncaughtExceptionsAsFatal = true;
        Crashlytics.IsCrashlyticsCollectionEnabled = true;
    }
}