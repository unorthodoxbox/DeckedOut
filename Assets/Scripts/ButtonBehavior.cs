using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ButtonBehavior : MonoBehaviour
{
    public void quitButton() {
        //hmmm
        Debug.Log("quit button pressed");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit ();
        #endif
    }

    public void playButton() {
        Debug.Log("play button pressed");
        SceneManager.LoadScene("NewEnvironment");
    }
}
