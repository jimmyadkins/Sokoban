using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuUI;  
    public GameObject levelCompleteUI;
    public GameObject inGameText;  
    private bool isPaused = false;  

    void Update()
    {
        if (!levelCompleteUI.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
            {
                if (isPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetScene();
        }
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true); 
        inGameText.SetActive(false); 
        Time.timeScale = 0f; 
        isPaused = true;  
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        inGameText.SetActive(true);  
        Time.timeScale = 1f; 
        isPaused = false; 
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu");  
    }

    public void ResetScene()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  
    }

    public void ShowLevelComplete()
    {
        levelCompleteUI.SetActive(true);  
        inGameText.SetActive(false); 
        //Time.timeScale = 0f; 
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f; 
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);  
        }
        else
        {
            Debug.Log("No more levels to load!");
            LoadMainMenu();
        }
    }
}
