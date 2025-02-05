using UnityEngine;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance { get; private set; }
    private UserData currentUser;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetCurrentUser(UserData userData)
    {
        currentUser = userData;
        // อัพเดท CurrentUser static class ด้วย
        CurrentUser.SetCurrentUser(userData);
        Debug.Log($"UserManager: Set current user - ID: {userData.userId}, Name: {userData.name}");
    }

    public UserData GetCurrentUser()
    {
        return currentUser;
    }
} 