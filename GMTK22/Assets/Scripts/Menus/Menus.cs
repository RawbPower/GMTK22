using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = GetComponent<AudioManager>();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        FindObjectOfType<GlobalAudioManager>().Play("Click");
    }

    public void QuitGame()
    {
        Application.Quit();
        FindObjectOfType<GlobalAudioManager>().Play("Click");
    }

    public void ReplayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        FindObjectOfType<GlobalAudioManager>().Play("Click");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        FindObjectOfType<GlobalAudioManager>().Play("Click");
    }

    public void HowToPlay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
        FindObjectOfType<GlobalAudioManager>().Play("Click");
    }
}
