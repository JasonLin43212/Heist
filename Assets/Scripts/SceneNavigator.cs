using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    public void RestartGame()
    {
        if (GameState.sceneName == null) {
            SceneManager.LoadScene(sceneName:"MainMenu");
        } else {
            SceneManager.LoadScene(sceneName:GameState.sceneName);
        }
    }

    public void StartGame(string sceneName) {
        GameState.sceneName = sceneName;
        SceneManager.LoadScene(sceneName:sceneName);
    }

    public void MainMenu() {
        SceneManager.LoadScene(sceneName:"MainMenu");
    }
}
