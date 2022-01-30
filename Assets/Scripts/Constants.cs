using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    private static readonly string[] RandomMessages =
    {
        "There's no time",
        "It wasn't supposed to end like this",
        "Everything's going to be fine",
        "It's almost like I can hear her",
        "She could be anywhere",
        "They are lurking in the shadows",
        "They don't like me",
        "They can't find me",
        "I must be quick",
        "Why can't I find her?",
        "I shouldn't have left her here",
        "Maybe I should just give up",
        "Where is she?",
        "Why did I do it?\nWhy..."
    };
    
    private static readonly string[] StartMessages =
    {
        "There's no time, I must find her",
        "I must find her, before it's too late"
    };
    
    public static string GetMessage()
    {
        string sentence = RandomMessages[Random.Range(0, RandomMessages.Length)];

        return sentence;
    }
    
    public static string GetStartMessage()
    {
        string sentence = StartMessages[Random.Range(0, StartMessages.Length)];

        return sentence;
    }
}
