using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {

	// Update is called once per frame
	void Update ()
	{
		
		// Check if any key is pressed
		if (Input.anyKeyDown)
		{
			// Load the scene with index 1 (i.e. the Tutorial Scene)
			SceneManager.LoadScene ("Tutorial", LoadSceneMode.Single);
		}
	}

}
