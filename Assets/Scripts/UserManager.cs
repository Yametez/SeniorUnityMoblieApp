using UnityEngine;

public class UserManager : MonoBehaviour
{
    private static UserManager instance;
    private UserData currentUser;

    // เพิ่ม public property สำหรับเข้าถึง instance
    public static UserManager Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        // Singleton pattern with DontDestroyOnLoad
        if (instance == null)
        {
            instance = this;
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