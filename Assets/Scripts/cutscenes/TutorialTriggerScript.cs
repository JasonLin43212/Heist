using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTriggerScript : GenericTriggerScript
{
    public GameObject tutorialPopupUI;
    private bool popupIsActive;

    void Start()
    {
        popupIsActive = false;
        transform.GetChild(0).gameObject.SetActive(false);  // Hide sprite
    }

    // Update is called once per frame
    void Update()
    {
        if(popupIsActive){
            if (Input.GetMouseButtonDown(0)){
                popupIsActive = false;
                tutorialPopupUI.SetActive(false);
                PauseMenu.popupIsEnabled = false;
                DestroySelf();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player"){
            popupIsActive = true;
            tutorialPopupUI.SetActive(true);
            this.GetComponent<BoxCollider2D>().enabled = false;
            PauseMenu.popupIsEnabled = true;
        }
    }
}
