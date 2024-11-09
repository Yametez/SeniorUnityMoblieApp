using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneModule : MonoBehaviour
{
    public string value;

    public void ChangeScene(){
        SceneManager.LoadScene(value, LoadSceneMode.Single);
    }
}
