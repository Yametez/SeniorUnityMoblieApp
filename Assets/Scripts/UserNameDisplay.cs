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
        }
    }
} 