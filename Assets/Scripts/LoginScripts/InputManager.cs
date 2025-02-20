using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    public InputField emailInput;

    void Start()
    {
        // หา InputField components โดยใช้ชื่อ GameObject
        emailInput = GameObject.Find("InputEmail").GetComponent<InputField>();
    }

    public void ClearEmailInput()
    {
        if (emailInput != null)
            emailInput.text = "";
    }
} 