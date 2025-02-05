using UnityEngine;
using UnityEngine.UI;

public class UserNameDisplay : MonoBehaviour
{
    private Text userNameText;

    void Start()
    {
        userNameText = GetComponent<Text>();
        if (userNameText != null)
        {
            UpdateUserName();
        }
    }

    void UpdateUserName()
    {
        UserData currentUser = CurrentUser.GetCurrentUser();
        if (currentUser != null)
        {
            userNameText.text = $"สวัสดี, คุณ{currentUser.name}";
            Debug.Log($"Current User ID: {currentUser.userId}");
            Debug.Log($"Current User Email: {currentUser.email}");
        }
        else
        {
            Debug.Log("No user currently logged in");
        }
    }
} 