using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneTriggerScript : MonoBehaviour
{
    public string cutscene;

    // Start is called before the first frame update
    void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);  // Hide sprite
        Debug.Assert(CampaignCutscenes.CutsceneExists(cutscene), "Specified cutscene isn't a valid cutscene identifier in CampaignCutscenes.");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            GameState.Instance.CutsceneController.StartCutscene(CampaignCutscenes.GetCutscene(cutscene));
            if (GetComponent<GoalTriggerScript>() != null) Destroy(this);
            else{
                Destroy(gameObject);
                GameState.Instance.GameController.pauseStopwatch();
            }

        }
    }
}
