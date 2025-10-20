using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private GameObject pauseMenu;
    public static bool isPaused;
    public AudioSource windAudio;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
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
    
    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f; // Freeze game time
        isPaused = true;
        SceneManager.UnloadSceneAsync("PlayerHUD");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (windAudio != null)
            windAudio.Pause();
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f; // Resume game time
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene("PlayerHUD", LoadSceneMode.Additive);

        if (windAudio != null)
            windAudio.UnPause();
    }

    public void Restart()
    {
        SceneManager.LoadScene("MasterScene");
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Additive);
        SceneManager.LoadScene("PlayerHUD", LoadSceneMode.Additive);
        SceneManager.LoadScene("PauseMenuScene", LoadSceneMode.Additive);
        SceneManager.LoadScene("Enviroment", LoadSceneMode.Additive);
        SceneManager.LoadScene("BossFight", LoadSceneMode.Additive);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f; // Ensure time scale is reset
    }

    public void Settings()
    {
        // Implement settings functionality here
        Debug.Log("Settings button clicked");
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("StartScreenScene");
        Time.timeScale = 1f;
    }
}
