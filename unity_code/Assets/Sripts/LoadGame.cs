using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
///<summary>
///
///<summary>
public class LoadGame : MonoBehaviour
{
    public Image mouse;
    private void Update()
    {
        mouse.rectTransform.position = Input.mousePosition;
    }
    public void LoadGameSecene()
    {
        SceneManager.LoadScene(1);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
