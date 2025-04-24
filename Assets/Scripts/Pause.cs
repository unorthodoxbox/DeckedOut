using System;
using UnityEngine;

public class Pause : MonoBehaviour
{
	bool paused = false;
    public GameObject pauseUI;

	void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
			paused = togglePause();
	}
	
	bool togglePause()
	{
		if(Time.timeScale == 0f)
		{
			Cursor.lockState = CursorLockMode.Locked;
        	Cursor.visible = false;
			Time.timeScale = 1f;
            pauseUI.SetActive(false);
			AudioListener.pause = false;
			return(false);
		}
		else
		{
			Time.timeScale = 0f;
			Cursor.lockState = CursorLockMode.None;
        	Cursor.visible = true;
            pauseUI.SetActive(true);
			AudioListener.pause = true;
			return(true);	
		}
	}
}