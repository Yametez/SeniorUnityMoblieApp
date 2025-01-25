using UnityEngine;
using UnityEngine.UI;
using System;

public class GoogleSignInManager : MonoBehaviour
{
    [SerializeField] private Button googleSignInButton;
    
    void Start()
    {
        if (googleSignInButton != null)
        {
            googleSignInButton.onClick.AddListener(SignInWithGoogle);
        }
    }

    private void SignInWithGoogle()
    {
        // จะเพิ่ม Google Sign-In logic ในขั้นตอนถัดไป
        Debug.Log("Google Sign-In Clicked");
    }
}