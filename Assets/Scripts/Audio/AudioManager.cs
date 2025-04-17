using System;
using System.Runtime.CompilerServices;
using UnityEngine;


public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    void Awake()
    {
        // Initialize all sound objects
        foreach(Sound sound in sounds) {
            if(sound.source == null) {
                 sound.source = gameObject.AddComponent<AudioSource>();
                 sound.source.playOnAwake = false;
            }
            sound.source.volume = sound.volume;
            if(sound.source.resource == null || sound.source.resource != sound.clip) {
                    sound.source.resource = sound.clip;    
            }   
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
          }
    } 

    public void Play(string name) {
        Sound s = GetSound(name);
        if (s == null) {
            return;
        }
        s.source.Play();
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
    
    private Sound GetSound(string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s == null) {
            Debug.LogWarning("Couldn't find Sound " +  name);
            return null;
        }
        return s;
    }
}
