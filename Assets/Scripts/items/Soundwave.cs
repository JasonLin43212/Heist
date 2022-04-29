using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soundwave : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator soundAnimation;
    void Start()
    {
        soundAnimation = GetComponent<Animator>();
    }

    public void setPosition(Vector2 pos) {
        transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        if (soundAnimation.GetCurrentAnimatorStateInfo(0).IsName("EmptySoundwave"))
        {
            Destroy(gameObject);
        }
    }
}
