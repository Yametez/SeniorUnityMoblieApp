public static class CurrentUser
{
    private static UserData _currentUser;

    public static void SetCurrentUser(UserData userData)
    {
        _currentUser = userData;
    }

    public static UserData GetCurrentUser()
    {
        return _currentUser;
    }
} 