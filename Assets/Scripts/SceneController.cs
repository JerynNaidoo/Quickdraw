using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("Scene Names")]
    public string masterScene = "Master";
    public string enviromentScene = "Enviroment";
    public string gameplayScene = "Gameplay";
    public string playerHudScene = "PlayerHUD";

    void Start()
    {
        // Load MasterScene first as the main environment
        SceneManager.LoadScene(masterScene, LoadSceneMode.Single);
        // Then load the other scenes on top
        SceneManager.LoadScene(enviromentScene, LoadSceneMode.Additive);

        SceneManager.LoadScene(gameplayScene, LoadSceneMode.Additive);

        SceneManager.LoadScene(playerHudScene, LoadSceneMode.Additive);
    }
}
