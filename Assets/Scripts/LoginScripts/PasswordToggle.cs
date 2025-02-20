using UnityEngine;
using UnityEngine.UI;

public class PasswordToggle : MonoBehaviour
{
    public InputField passwordInput;
    public Image toggleButtonImage;    // Reference ไปยัง Image component ของปุ่ม
    public Sprite eyeSprite;          // รูป eye.png
    public Sprite hideSprite;         // รูป hide.png
    
    private bool isPasswordVisible = false;

    void Start()
    {
        passwordInput = GameObject.Find("InputPassword").GetComponent<InputField>();
        // ตั้งค่าเริ่มต้นเป็นรูป hide
        if (toggleButtonImage != null)
            toggleButtonImage.sprite = hideSprite;
    }

    public void TogglePassword()
    {
        isPasswordVisible = !isPasswordVisible;
        
        // สลับการแสดงรหัสผ่าน
        passwordInput.contentType = isPasswordVisible ? 
            InputField.ContentType.Standard : 
            InputField.ContentType.Password;
            
        // เปลี่ยนรูปไอคอน
        toggleButtonImage.sprite = isPasswordVisible ? 
            eyeSprite : hideSprite;
            
        // รีเฟรช input field
        passwordInput.ForceLabelUpdate();
    }
} 