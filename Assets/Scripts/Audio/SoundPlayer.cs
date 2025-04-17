using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;
public enum Manner {
    FREEZE, // SoundPlayer will play the same sound in Sounds
    ORDER, // SoundPlayer will play the sounds in order
    SHUFFLE, // SoundPlayer will shuffle randomly through sounds


}

// A component meant mainly for periodic emission of SFX from a Game Object. 
public class SoundPlayer : MonoBehaviour
{
    AudioSource audioSource;
    AudioManager audioManager;
    public Manner manner = Manner.FREEZE;

    // Time between audio plays
    public float delay;
    private float timer;
    
    // By how much do you want the delay to vary between plays?
    public float stagger = 0; 

    public string[] sounds;
    int currentSound = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        timer = delay + Random.Range(-stagger, stagger);
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();
        if(audioSource == null) {
            audioSource = gameObject.AddComponent<AudioSource>(); 
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer < 0) {        
            audioManager.PlayFromSource(sounds[currentSound], audioSource);
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
            timer = delay + Random.Range(-stagger, stagger);
            if(timer <= 0) {
                timer = delay;
            }
        }
    }

    public void ChangeManner(Manner manner) {
        this.manner = manner;
    }
}
