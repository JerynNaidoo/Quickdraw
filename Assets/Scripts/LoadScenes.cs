using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;



public class StartScreen : MonoBehaviour
{
    [Header("Scenes to Load")]
    public string[] scenesToLoad; // List of scene names
    public GameObject startScreenCanvas;
    [Header("Optional Delay")]
    public float fadeDuration = 1f; // If you want a fade

    public void PlayGame()
    {
        StartCoroutine(LoadScenes());
    }

    private IEnumerator LoadScenes()
    {
        foreach (string sceneName in scenesToLoad)
        {
            if (!SceneManager.GetSceneByName(sceneName).isLoaded)
            {
                yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            }
        }

        // Wait a frame to ensure all scenes initialize
        yield return null;

        // Set the active scene (important for camera, lighting, etc.)
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scenesToLoad[0]));

        // Now it's safe to unload the start screen
        startScreenCanvas.SetActive(false);
        SceneManager.UnloadSceneAsync("StartScreenScene");
        
    }


    // Optional fade coroutine
    /*
    private IEnumerator FadeOut()
    {
        // Implement black panel fade logic here
        yield return new WaitForSeconds(fadeDuration);
    }
    */
}
