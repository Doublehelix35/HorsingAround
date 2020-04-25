using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public string SceneToLoad;

    public void LoadNextScene()
    {
        SceneManager.LoadScene(SceneToLoad);
    }

    public void ToggleThis()
    {
        gameObject.SetActive(!gameObject.activeInHierarchy);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
