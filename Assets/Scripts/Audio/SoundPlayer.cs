using System;
using System.Diagnostics;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;
public enum Manner {
    STOP, // SoundPlayer will only play when Play is called
    FREEZE, // SoundPlayer will play the same sound in Sounds
    ORDER, // SoundPlayer will play the sounds in order
    SHUFFLE, // SoundPlayer will shuffle randomly through sounds


}

// A component meant mainly for periodic emission of SFX from a Game Object. 
public class SoundPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    AudioManager audioManager;
    public Manner manner = Manner.FREEZE;

    // Time between audio plays
    public float delay;
    private float timer;
    
    // By how much do you want the delay to vary between plays?
    public float stagger = 0; 

    public string[] sounds;
    int currentSound = 0;
    bool playFromSource = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        timer = Random.Range(0, delay);
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();
        if(audioSource == null) {
            audioSource = gameObject.AddComponent<AudioSource>(); 
        }
        
        if(gameObject.name == "Music Player") {
            sounds = new string[audioManager.music.Length];
            for(int i = 0; i < audioManager.music.Length; i++) {
                sounds[i] = audioManager.music[i].name;
            }
            playFromSource = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(audioSource.isPlaying == true || manner == Manner.STOP || sounds.Length == 0) {
            return;    
        }
        timer -= Time.deltaTime;
        if(timer < 0) {        
            if(playFromSource) {
                audioManager.PlayFromSource(sounds[currentSound], audioSource);
            } else {
                audioManager.Play(sounds[currentSound]);
            }
            SelectSound();
            timer = delay + Random.Range(-stagger, stagger);
            if(timer <= 0) {
                timer = delay;
            }
        }
    }

    public void ChangeManner(Manner manner) {
        this.manner = manner;
    }

    private void SelectSound() {
        switch(manner) {
            case Manner.FREEZE:
                break;
            case Manner.ORDER:
                currentSound++;
                if(currentSound >= sounds.Length){
                    currentSound = 0;
                }
                break;
            case Manner.SHUFFLE:
                currentSound = Random.Range(0, sounds.Length);
                break;
            }
    }
    // Play() plays a oneshot from the provided source.
    public void Play() {
        audioManager.PlayFromSource(sounds[currentSound], audioSource);
        SelectSound();
    }

    public void Play(string name) {
        audioManager.PlayFromSource(name, audioSource);
    }
    
    public void Play(int index) {
        audioManager.PlayFromSource(sounds[index], audioSource);
    }
}
