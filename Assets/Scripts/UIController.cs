using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class UIController : MonoBehaviour
{
    public static UIController Singleton;
    
    [FormerlySerializedAs("target")]
    public TextMeshProUGUI typewriteTarget = null;
    public AudioClip typewriteClip;
    public AudioClip newlineClip;
    public bool uiUpdating = false;

    
    public TextMeshProUGUI openDoorText;
    public Transform endPanel;
    public TextMeshProUGUI endText;
    public float startTextFadeSpeed = 2;

    public Image damageImage;
    public float damageFadeSpeed = 3;

    private AudioSource audioSource;

    public enum OutputMode
    {
        Additive = 0,
        Replace = 1,
        FromRandom = 2,
        FromRandomAndRemove = 3,
        GoBackAndReplace = 4
    }

    private enum SoundType
    {
        Typewrite = 0,
        Newline = 1
    }

    private void Awake()
    {
        if (Singleton != null)
        {
            Destroy(gameObject);
            return;
        }
        Singleton = this;
        
        openDoorText.gameObject.SetActive(false);
        damageImage.gameObject.SetActive(false);
        
        audioSource = GetComponent<AudioSource>();
        
        SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
    }

    public IEnumerator Typewrite(string targetText, OutputMode mode, float printDelay, float startDelay)
    {
        uiUpdating = true;
        char[] chars = targetText.ToCharArray();
        Coroutine coroutine = null;

        yield return new WaitForSeconds(startDelay);

        switch (mode)
        {
            case OutputMode.Additive:
                {
                    coroutine = StartCoroutine(TypewriteSound());
                    for (int i = 0; i < chars.Length; i++)
                    {
                        typewriteTarget.text += chars[i];
                        yield return new WaitForSeconds(printDelay);
                    }
                    StopCoroutine(coroutine);

                    yield return new WaitForSeconds(printDelay * 20);

                    char[] charArray = targetText.ToCharArray();
                    for (int i = 0; i < charArray.Length; i++)
                    {
                        typewriteTarget.text = typewriteTarget.text.Remove(typewriteTarget.text.Length - 1);
                        yield return new WaitForSeconds(printDelay / 2);
                    }
                }
                break;

            case OutputMode.Replace:
                {
                    coroutine = StartCoroutine(TypewriteSound());
                    typewriteTarget.text = "";
                    for (int i = 0; i < chars.Length; i++)
                    {
                        typewriteTarget.text += chars[i];
                    }
                }
                break;

            case OutputMode.FromRandom:
                {
                    coroutine = StartCoroutine(TypewriteSound());
                    typewriteTarget.text = "";

                    for (int i = 0; i < chars.Length; i++)
                    {
                        //set the string to random chars
                        typewriteTarget.text = GetRandomChars(chars.Length);

                        //modify the chars we have already written
                        StringBuilder sb = new StringBuilder(typewriteTarget.text);
                        for (int j = 0; j < i + 1; j++)
                        {
                            sb[j] = chars[j];
                        }
                        typewriteTarget.text = sb.ToString();
                        yield return new WaitForSeconds(printDelay);
                    }
                }
                break;

            case OutputMode.FromRandomAndRemove:
                {
                    coroutine = StartCoroutine(TypewriteSound());
                    typewriteTarget.text = "";

                    for (int i = 0; i < chars.Length; i++)
                    {
                        //set the string to random chars
                        typewriteTarget.text = GetRandomChars(chars.Length);

                        //modify the chars we have already written
                        StringBuilder sb = new StringBuilder(typewriteTarget.text);
                        for (int j = 0; j < i + 1; j++)
                        {
                            sb[j] = chars[j];
                        }
                        typewriteTarget.text = sb.ToString();
                        yield return new WaitForSeconds(printDelay);
                    }
                    StopCoroutine(coroutine);

                    yield return new WaitForSeconds(printDelay * 20);

                    char[] charArray = targetText.ToCharArray();
                    for (int i = 0; i < charArray.Length; i++)
                    {
                        typewriteTarget.text = typewriteTarget.text.Remove(typewriteTarget.text.Length - 1);
                        yield return new WaitForSeconds(printDelay / 2);
                    }
                }
                break;

            case OutputMode.GoBackAndReplace:
                {
                    //yield return new WaitForSeconds(printDelay * 2);
                    int len = typewriteTarget.text.Length;
                    for (int i = 0; i < len; i++)
                    {
                        yield return new WaitForSeconds(printDelay / 2);
                        typewriteTarget.text = typewriteTarget.text.Remove(typewriteTarget.text.Length - 1);
                    }

                    yield return new WaitForSeconds(printDelay * 2);

                    coroutine = StartCoroutine(TypewriteSound());

                    for (int i = 0; i < chars.Length; i++)
                    {
                        yield return new WaitForSeconds(printDelay / 2);
                        typewriteTarget.text += chars[i];
                    }
                }
                break;

            default:
                yield return null;
                break;
        }
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }

        uiUpdating = false;
    }

    IEnumerator TypewriteSound()
    {
        while (true && typewriteClip != null)
        {
            audioSource.pitch = Random.Range(0.8f, 1.2f);
            audioSource.PlayOneShot(typewriteClip);
            yield return new WaitForSeconds(Random.Range(0.1f, 0.25f));
        }
    }

    private string GetRandomChars(int amount)
    {
        var random = new System.Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789#";

        return new string(Enumerable.Repeat(chars, amount).Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private void PlaySound(SoundType type)
    {
        switch (type)
        {
            case SoundType.Typewrite:
                audioSource.pitch = Random.Range(0.8f, 1.2f);
                audioSource.clip = typewriteClip;
                break;
            case SoundType.Newline:
                audioSource.clip = newlineClip;
                break;
            default:
                break;
        }
        audioSource.PlayOneShot(audioSource.clip);
    }

    public void EnableOpenDoorText(KeyCode openKey)
    {
        openDoorText.text = $"Open door ({openKey.ToString()})";
        openDoorText.gameObject.SetActive(true);
    }

    public void DisableOpenDoorText()
    {
        openDoorText.gameObject.SetActive(false);
    }

    public void EnableTalkText(KeyCode openKey)
    {
        openDoorText.text = $"Talk to: Her ({openKey.ToString()})";
        openDoorText.gameObject.SetActive(true);
    }

    public void DisableTalkText()
    {
        openDoorText.gameObject.SetActive(false);
    }

    public void EnableEndPanel(string endText)
    {
        this.endText.text = endText;
        endPanel.gameObject.SetActive(true);
    }

    public void TriggerStartSequence()
    {
        endText.text = "Postmortem";
        StartCoroutine(FadePanel());
        GameController.Singleton.inEnd = false;
    }

    private void SceneManagerOnsceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        DisableOpenDoorText();
        DisableTalkText();
    }

    private IEnumerator FadePanel()
    {
        CanvasGroup canvasGroup = endPanel.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        while (canvasGroup.alpha > 0.001f)
        {
            canvasGroup.alpha -= Time.deltaTime * startTextFadeSpeed;
            yield return null;
        }

        endPanel.gameObject.SetActive(false);
        canvasGroup.alpha = 1;
    }
    
    public IEnumerator FadeDamage()
    {
        damageImage.color = new Color(damageImage.color.r, damageImage.color.b, damageImage.color.b, 1);
        damageImage.gameObject.SetActive(true);
        while (damageImage.color.a > 0.001f)
        {
            damageImage.color = new Color(damageImage.color.r, damageImage.color.b, damageImage.color.b, damageImage.color.a - Time.deltaTime * damageFadeSpeed);
            yield return null;
        }

        damageImage.gameObject.SetActive(false);
    }
}
