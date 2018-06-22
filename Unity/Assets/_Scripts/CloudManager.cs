using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour 
{

	// Holds all the effects selected tobe placed in the scene
	public List<Effect> CloudEffects;

	public KeyCode[] keys =
	{
		KeyCode.W,
		KeyCode.E,
		KeyCode.R,
		KeyCode.T,
		KeyCode.Y,
		KeyCode.U,
		KeyCode.I,
		KeyCode.O,
	};

	private CloudStream[] objects;
	private GlobalWind windEffect;
	public float windHeight = 2.5f;
	// Use this for initialization
	void Start() 
	{
		// Create a array to hold the number of possible effects
		objects = new CloudStream[CloudEffects.Count]; 

		// temp counter
		int counter = 0;

		// Check that all the Effects are valid
		foreach(var effect in CloudEffects)
		{
			objects [counter] = effect.SpawnEffect(GetStartParticlePosition(counter, CloudEffects.Count));
			counter++;
		}

		windEffect = new GlobalWind(objects);
	}

	// Update is called once per frame
	void Update()
	{

		// if an input is fired check what effects should be activated
		if (Input.anyKeyDown)
		{

			for (int i = 0; i < objects.Length; ++i)
			{
				if (Input.GetKeyDown(keys[i]))
				{
					objects[i].Play();
				}
			}

		}

		// if an input is released check what effects should be deactivated

		for (int i = 0; i < objects.Length; ++i)
		{
			if (objects[i].musicSource.isPlaying)
			{
				if (!Input.GetKey(keys[i]))
				{
					objects[i].Stop();
				}
			}
		}

		windEffect.windHeight = windHeight;
		windEffect.Update();

		float[] spectrum = new float[256];

		AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

		int[] largest =
		{
			0,
			0,
			0
		};
		float[] large_f =
		{
			0.0f,
			0.0f,
			0.0f
		};
		for (int j = 0; j < 3; ++j)
		{
			for (int i = j * 256 / 3; i < (j + 1) * 256 / 3; ++i)
			{
				if (large_f[j] < spectrum[i])
				{
					largest[j] = i;
					large_f[j] = spectrum[i];
				}
			}
		}
		
		Color newColour = new Color(largest[0] / 10.0f, largest[1] / 10.0f, largest[2] / 10.0f, 1.0f);
		foreach (var obj in objects)
		{
			var main = obj.particles.main;
			main.startColor = newColour;
		}
	}


	// Function to return the position of a particle system based the number of systems and it's place in the list
	Vector3 GetStartParticlePosition(int position, int totalNumberofSystems)
	{

		// Get the main Camerea W value
		float ScreenHeightUnits = Camera.main.orthographicSize * 2.0f;
		float ScreenWidthUnits = ScreenHeightUnits * Screen.width / Screen.height;

		// Catch for one system
		if (totalNumberofSystems == 1)
			return new Vector3 (0.0f, -5.0f, 0.0f);

		// the y and z positions are fixed as the visuals are 3d 
		float denomimator = totalNumberofSystems + 1;

		if (denomimator == 0.0f)
			return new Vector3 (0.0f, 0.0f, 0.0f);

		// Calculate the positions 
		float xpositions = -(0.5f * ScreenWidthUnits) + ((float)position + 1.0f) * ScreenWidthUnits / denomimator;

		// Return the positions relative to the number of systems
		return new Vector3(xpositions, -5.0f, 0.0f);
	
	}


}
