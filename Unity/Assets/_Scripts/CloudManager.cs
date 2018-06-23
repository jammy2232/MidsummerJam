using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour 
{

	// Holds all the effects selected to be placed in the scene
	public List<Effect> CloudEffects;

	// This array should be the same length as num cloud effects
	// each key will be tied to a sound
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

	// Anything above this height will be affected by wind.
	public float windHeight = 2.5f;
	// This also controls how powerful the wind is
	public float windDirection = -1.0f;

	// We perform fourier transforms on the audio to get the frequency of the audio.
	// This is then used to change the colour of particles. To make the colour changes
	// more gradual you can increase the number of buckets. This must be a multiple of 2
	// see AudioListener.GetSpectrumData for all of the constraints
	public int FrequencyBuckets = 64;

	// Use this for initialization
	void Start() 
	{
		// Create a array to hold the number of possible effects
		objects = new CloudStream[CloudEffects.Count]; 

		int counter = 0;
		foreach(var effect in CloudEffects)
		{
			// Equally space all the effects across the screen and init them
			objects[counter] = effect.SpawnEffect(GetStartParticlePosition(counter, CloudEffects.Count));
			counter++;
		}

		// In an ideal world this would have been more Unityy but I'm used to C++...
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

		float[] spectrum = new float[FrequencyBuckets];

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
			for (int i = j * FrequencyBuckets / 3; i < (j + 1) * FrequencyBuckets / 3; ++i)
			{
				if (large_f[j] < spectrum[i])
				{
					largest[j] = i - j*FrequencyBuckets/3;
					large_f[j] = spectrum[i];
				}
			}
		}

		float total = largest[0] + largest[1] + largest[2] + 1;

		Color newColour = new Color(largest[0]/total, largest[1]/total, largest[2]/total, 1.0f);

		foreach (var obj in objects)
		{
			var main = obj.particles.main;
			main.startColor = newColour;
		}

		windEffect.windDirection = (largest[0] - largest[1])/total;
		windEffect.windHeight = windHeight;
		windEffect.Update();
	}


	// Generate a start position for particles so that they are equally spaced for the
	// number of particle generators.
	Vector3 GetStartParticlePosition(int position, int totalNumberofSystems)
	{
		// Get the main Camerea W value
		float ScreenHeightUnits = Camera.main.orthographicSize * 2.0f;
		float ScreenWidthUnits = ScreenHeightUnits * Screen.width / Screen.height;

		// Catch for one system
		if (totalNumberofSystems == 1)
			return new Vector3 (0.0f, 0.0f, 0.0f);

		// the y and z positions are fixed as the visuals are 3d 
		float denomimator = totalNumberofSystems + 1;

		if (denomimator == 0.0f)
			return new Vector3 (0.0f, 0.0f, 0.0f);

		// Calculate the positions 
		float xpositions = -(0.5f * ScreenWidthUnits) + ((float)position + 1.0f) * ScreenWidthUnits / denomimator;

		// Return the positions relative to the number of systems
		return new Vector3(xpositions, -2.0f, 0.0f);
	}
}
