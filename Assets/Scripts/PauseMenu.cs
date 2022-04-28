using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public static bool isGamePaused = false;
    public GameObject pauseMenuUI;

    void Start()
    {
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
        Time.timeScale = (isGamePaused || GameState.Instance.CutsceneController.InCutscene) ? 0f : 0.5f;
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
