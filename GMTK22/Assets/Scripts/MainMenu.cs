using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = GetComponent<AudioManager>();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        audioManager.Play("Click");
    }
    public void QuitGame()
    {
        Application.Quit();
        audioManager.Play("Click");
    }

    public void HowToPlay()
    {
        Application.Quit(SceneManager.GetActiveScene().buildIndex + 2);
        audioManager.Play("Click");
    }
}
