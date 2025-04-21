using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;



public class AudioManager : MonoBehaviour
{
    // All purely organizational
    [Header("Music")]
    [Tooltip("BGM")]
    public Sound[] music; 
    public int currentSong = 0; // Index in Music of the current song. Set to -1 if No Music Should Play
    [Range(0f, 3f)]
    public float musicVolume = .5f; // Universal music volume

    [Header("UI")]
    [Range(0f, 3f)]
    public float uiVolume = .5f;
    [Tooltip("Sounds produced by UI")]
    public Sound[] ui;

    [Header("Sounds")]
    [Tooltip("Sounds attached to the player (Collecting Cards, Getting Hit)")]
    public Sound[] player; 
    
    [Tooltip("Sounds produced by entities other than the player--the kinds of things played by SoundPlayers")]
    public Sound[] sfx; // for external SFX like bugs hissing, cards humming

    private Sound[] sounds;

    public static AudioManager instance;

    void Awake()
    {
        /* if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject); */


        sounds = music.Concat(player).Concat(ui).Concat(sfx).ToArray();

        // Initialize all sound objects
        foreach(Sound sound in sounds) {
            if(sound.category == Category.OTHER) {
                    //sound.source.volume = sound.volume;
            } else {
                switch(sound.category) {
                    case Category.MUSIC:
                        sound.source.volume = musicVolume;
                        break;
                    case Category.UI: 
                        sound.source.volume = uiVolume;
                        break;
                }
            } 
            
        }

    }

    // Plays a given sound. Replaces an audiosource's clip.
    public void Play(Sound s) {
        if(ErrorCheck(s, true, "Play") < 0) {
            return;
        }
        if(s.source != null) {
                s.source.loop = s.loop;
        }
        if(s.source.resource != s.clip) {
            s.source.resource = s.clip;
        }
        s.source.Play();
    }
    public void Play(string name) {
        Play(GetSound(name));
    }

    // Plays a sound once regardless of if it's set to loop
    public void PlayOneShot(Sound s) {
        if(ErrorCheck(s, true, "PlayOneShot") < 0) {
            return;
        }      
        s.source.PlayOneShot(s.clip, s.volume);
    }

    public void PlayOneShot(string name) {
        PlayOneShot(GetSound(name));
    }

    // Plays the sound's clip from the provided AudioSource without permanently changing 
    // the sound's source or the AudioSource's clip.
     public void PlayFromSource(Sound s, AudioSource source) {
        if(ErrorCheck(s, false, "PlayFromSource") < 0) {
            return;
        };
        if(source == null) {
            Debug.LogWarning("Can't PlayFromSource. " + source.name + "is null.");
            return;
        }
        source.PlayOneShot(s.clip, s.volume);
     }

    public void PlayFromSource(String name, AudioSource source) {
        PlayFromSource(GetSound(name), source);
     }

    // Not overloading these until it's required. 
    public void Stop(string name) {
        Sound s = GetSound(name);
        if(s == null) {
            return;
        }
        s.source.Stop();
    }

    // When pause is true, pauses the Sound named name. When pause is false, unpauses.
    public void Pause(string name, bool pause)  {
        Sound s = GetSound(name);
        if(s == null ) {
            return;
        }
        if(pause) {
            s.source.Pause();
        } else {
            s.source.UnPause();
        }
    }


    
    public Sound GetSound(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null) {
            Debug.LogWarning("Couldn't find Sound " +  name);
            return null;
        }
        return s;
    }

    private int ErrorCheck(Sound s, bool checkSource, string op) {    
        int status = 0;
        if(s == null) {
            Debug.LogWarning("Can't " + op + ". " + s.name + " is null.");
            return -1;
        }
        if(s.clip == null) {
            Debug.LogWarning("Can't " + op + ". " + s.name + " has a null clip.");
            status--;
        }
        if(checkSource && s.source == null) {
            Debug.LogWarning("Can't " + op + ". " + s.name + " has a null source.");
            status--;
        }
        return status;
    }

}
