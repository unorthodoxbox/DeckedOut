using UnityEngine.Audio;
using UnityEngine;
using System;
using System.ComponentModel;

public enum Category {
    OTHER,
    MUSIC,
    UI
}

// I intend for this to eventually be a ScriptableObject so we don't need to use Strings everywhere.
// I attempted to implement this but it was just. really messy. Another time.
[System.Serializable]

public class Sound {


    public string name;
    public AudioSource source;
    public AudioClip clip;
    public Category category;

    [Range(0f, 2f)]
    public float volume = 1;
    [Range(0f, 3f)]
    public float pitch = 1;
    public bool loop;
    

}