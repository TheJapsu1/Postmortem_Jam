using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class Door : MonoBehaviour
{
    public string nextScene = "level_";

    public KeyCode openKey = KeyCode.E;

    private bool inOpenRadius;

    private void OnTriggerEnter2D(Collider2D other)
    {
        inOpenRadius = true;
        
        UIController.Singleton.EnableOpenDoorText(openKey);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        inOpenRadius = false;
        
        UIController.Singleton.DisableOpenDoorText();
    }

    private void Update()
    {
        if (inOpenRadius && Input.GetKeyDown(openKey))
        {
            //TODO: Fade sequence
            SceneManager.LoadScene(nextScene);
        }
    }
}
