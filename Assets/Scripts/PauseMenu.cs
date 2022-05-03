using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool isGamePaused = false;
    public GameObject pauseMenuUI;
    public static bool popupIsEnabled;

    void Start()
    {
        popupIsEnabled = false;
        Resume();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isGamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
        Time.timeScale = (  isGamePaused 
                            || GameState.Instance.CutsceneController.InCutscene
                            || GameState.Instance.GameController.playersHaveLost
                            || popupIsEnabled) ? 0f : .5f;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        isGamePaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        isGamePaused = true;
    }

    public static bool PausedByCutscene => GameState.Instance.CutsceneController.InCutscene;
}
