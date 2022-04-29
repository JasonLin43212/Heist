using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericTriggerScript : MonoBehaviour
{
    private string uniqueIdentifier;
    public string UniqueID => uniqueIdentifier;

    // Start is called before the first frame update
    void Start()
    {
        uniqueIdentifier = $"Trigger:{transform.position.ToString()},{transform.eulerAngles.z},{transform.localScale.ToString()}";
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
