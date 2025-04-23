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
			Time.timeScale = 1f;
            pauseUI.SetActive(false);
			AudioListener.pause = false;
			return(false);
		}
		else
		{
			Time.timeScale = 0f;
            pauseUI.SetActive(true);
			AudioListener.pause = true;
			return(true);	
		}
	}
}