using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class StepManager : MonoBehaviour
{
    public AudioClip stepClip;
    
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void Play()
    {
        StartCoroutine(StepSound());
    }

    private IEnumerator StepSound()
    {
        source.pitch = Random.Range(0.8f, 1.2f);
        source.PlayOneShot(stepClip);

        yield return null;
    }
}
