using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(BoxCollider2D))]
public class Bed : MonoBehaviour
{
    public KeyCode interactKey = KeyCode.E;

    private bool inInteractRadius;

    private void OnTriggerEnter2D(Collider2D other)
    {
        inInteractRadius = true;
        
        UIController.Singleton.EnableTalkText(interactKey);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        inInteractRadius = false;
        
        UIController.Singleton.DisableTalkText();
    }

    private void Update()
    {
        if (inInteractRadius && Input.GetKeyDown(interactKey))
        {
            //TODO: Fade sequence
            GameController.Singleton.OnGameFinish();
        }
    }
}
