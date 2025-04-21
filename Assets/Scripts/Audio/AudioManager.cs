using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;


public class AudioManager : MonoBehaviour
{
    // All purely organizational
    [Tooltip("BGM")]
    public Sound[] music; 
    [Tooltip("Sounds attached to the player (Collecting Cards, Getting Hit)")]
    public Sound[] player; 
    [Tooltip("Sounds produced by UI")]
    public Sound[] ui;
    [Tooltip("Sounds produced by entities other than the player--the kinds of things played by SoundPlayers")]
    public Sound[] sfx; // for external SFX like bugs hissing, cards humming


    private Sound[] sounds;

    public static AudioManager instance;

    void Awake()
    {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        sounds = music.Concat(player).Concat(ui).Concat(sfx).ToArray();
        // Initialize all sound objects
        foreach(Sound sound in sounds) {
            if(sound.source == null) {
                 sound.source = gameObject.AddComponent<AudioSource>();
                 sound.source.playOnAwake = false;
            }
            sound.source.volume = sound.volume;
            if(sound.source.resource == null /*|| sound.source.resource != sound.clip*/) {
                    sound.source.resource = sound.clip;    
            }   
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
        Play(sounds[0]); // Should be music.
    } 

    // Plays a given sound
    public void Play(Sound s) {
        if (s == null) {
            return;
        }
        if(s.source.resource != s.clip) {
            s.source.resource = s.clip;
         }
        s.source.Play();
    }
    public void Play(string name) {
        Sound s = GetSound(name);
        Play(s);
    }

    // Plays a sound once regardless of if it's set to loop
    public void PlayOneShot(string name) {
        Sound s = GetSound(name);
        if (s == null) {
            return;
        }
                
        s.source.PlayOneShot(s.clip, s.volume);
    }

    // Plays the sound's clip from the provided AudioSource without permanently changing 
    // the sound's source or the AudioSource's clip.
     public void PlayFromSource(string name, AudioSource source) {
        Sound s = GetSound(name);
        if (s == null) {
            return;
        }
        source.PlayOneShot(s.clip, s.volume);
     }

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
}
