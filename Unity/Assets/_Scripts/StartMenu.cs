using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {

	void Start()
	{
		// Hide the cursor
		Cursor.visible = false;
	}

	// Update is called once per frame
	void Update ()
	{

		// Quit
		if (Input.GetKeyDown (KeyCode.Escape))
		{
			Application.Quit ();
		}

		// Check if any key is pressed
		if (Input.anyKeyDown)
		{
			// Load the scene with index 1 (i.e. the Tutorial Scene)
			SceneManager.LoadScene ("Tutorial", LoadSceneMode.Single);
		}
	}

}
