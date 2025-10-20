using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public void Show()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Restart button clicked!");
        gameObject.SetActive(false);

        HealthBarManager hbm = FindObjectOfType<HealthBarManager>();
        if (hbm != null) hbm.ResetHealth();

        SceneManager.LoadScene("MasterScene", LoadSceneMode.Single);
        SceneManager.LoadScene("Enviroment", LoadSceneMode.Additive);
        SceneManager.LoadScene("Gameplay", LoadSceneMode.Additive);
        SceneManager.LoadScene("PlayerHUD", LoadSceneMode.Additive);
        SceneManager.LoadScene("PauseMenuScene", LoadSceneMode.Additive);
        //SceneManager.LoadScene("Loader");
        SceneManager.LoadScene("BossFight", LoadSceneMode.Additive);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
