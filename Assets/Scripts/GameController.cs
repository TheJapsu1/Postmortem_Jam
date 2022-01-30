using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController Singleton;
    
    [FormerlySerializedAs("textController")] [Header("Text stuff")]
    public UIController uiController;
    public int randomTextDelay = 80;
    public float randomTextTimeVariation = 20f;

    [Header("Finish stuff")]
    //public float nukeTime = 30f;
    public float safeTime = 20f;

    private float timeSinceLastText;
    private bool startTextShown = false;

    [SerializeField]
    private float gameTime;

    public bool inEnd = false;

    public int health = 100;
    private int startHealth;

    private string startScene;

    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;

        startHealth = health;

        startScene = SceneManager.GetActiveScene().name;
    }

    private void Start()
    {
        uiController.TriggerStartSequence();
    }
 
    public void Damage(int amount)
    {
        health -= amount;
        Debug.Log("Damage " + amount);
        StartCoroutine(UIController.Singleton.FadeDamage());
    }

    private void Update()
    {
        if (inEnd)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(startScene);
                uiController.TriggerStartSequence();

                health = startHealth;
                gameTime = 0;
            }
            
            return;
        }

        if (health <= 0)
        {
            SceneManager.LoadScene(startScene);
            uiController.TriggerStartSequence();

            health = startHealth;
            gameTime = 0;
        }
        
        if (!startTextShown && Time.time > 3)
        {
            StartCoroutine(uiController.Typewrite(Constants.GetStartMessage(), UIController.OutputMode.Additive, .1f, 0f));

            startTextShown = true;
        }
        
        timeSinceLastText += Time.deltaTime;
        
        if(timeSinceLastText > (randomTextDelay + Random.Range(-randomTextTimeVariation, randomTextTimeVariation))/* && !playerController.dead*/)
        {
            timeSinceLastText = 0;
            StartCoroutine(uiController.Typewrite(Constants.GetMessage(), UIController.OutputMode.FromRandomAndRemove, .1f, 0f));
        }

        gameTime += Time.deltaTime;
    }

    public void OnGameFinish()
    {
        Singleton.inEnd = true;
        
        if (gameTime < safeTime)
        {
            // Escape
            uiController.EnableEndPanel("You take her hand and help her out of the bed.\nAs you two are walking towards the door to leave,\nyou gaze through the window only to see a blinding light...\n\n[R] to find the other endings.");
        }
        else
        {
            // Nuke
            uiController.EnableEndPanel("As you reach your hand towards her, you can feel\nthe ground rumbling under you.\nThe building suddenly shifts and\nsends you flying across the room.\nYou try to get back to her, but the path is blocked.\nYou leave her behind.\n\n[R] to find the other endings.");
        }
        //TODO: Nuke & escape
    }
}
