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
	public const int FrequencyBuckets = 64;

	// Place to store the specturum for Frequency Data Analysis
	private float[] spectrum;

	// Use this for initialization
	void Start() 
	{
		// Create a array to hold the number of possible effects
		objects = new CloudStream[CloudEffects.Count]; 

		// temp counter
		int counter = 0;

		foreach(var effect in CloudEffects)
		{
			// Equally space all the effects across the screen and init them
			objects[counter] = effect.SpawnEffect(GetStartParticlePosition(counter, CloudEffects.Count));
			counter++;
		}

		// In an ideal world this would have been more Unityy but I'm used to C++...
		windEffect = new GlobalWind(objects);
		// To save on GC, only allocate this once
		spectrum = new float[FrequencyBuckets];
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

		// Split frequencies into 3 segments for RGB of colour and get the greatest component frequency
		int[] largestFrequencyBucket     = { 0,    0,    0 };
		float[] magnitudeOfLargestBucket = { 0.0f, 0.0f, 0.0f };
		PerformFrequencyAnalysis(largestFrequencyBucket, magnitudeOfLargestBucket);

		// Normalising factor to emphasise change (+0.00001 to prevent divide by zero)
		float total = largestFrequencyBucket[0] + largestFrequencyBucket[1] + largestFrequencyBucket[2] + 0.000001f;

		// Create a new particle colour based on the frequency
		Color newColour = new Color(
			largestFrequencyBucket[0] / total,
			largestFrequencyBucket[1] / total,
			largestFrequencyBucket[2] / total,
			1.0f);

		foreach (var obj in objects)
		{
			var particleEmitter	       = obj.particles.main;
			particleEmitter.startSize  = largestFrequencyBucket[0] / total * 30.0f;
			particleEmitter.startSpeed = largestFrequencyBucket[1] / total * 30.0f;
			particleEmitter.startColor = newColour;
		}

		windEffect.windDirection = (largestFrequencyBucket[0] - largestFrequencyBucket[1])/total;
		windEffect.windHeight = windHeight;
		windEffect.Update();
	}

	void PerformFrequencyAnalysis(int[] largestFrequencyBucket, float[] magnitudeOfLargestBucket)
	{
		AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Triangle);

		for (int j = 0; j < 3; ++j)
		{
			for (int i = j * FrequencyBuckets / 3; i < (j + 1) * FrequencyBuckets / 3; ++i)
			{
				if (magnitudeOfLargestBucket[j] < spectrum[i])
				{
					largestFrequencyBucket[j] = i - j * FrequencyBuckets / 3;
					magnitudeOfLargestBucket[j] = spectrum[i];
				}
			}
		}
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


		// Define a random height 
		float randomHeight = Random.Range(-2.0f, 0.0f);

		if (denomimator == 0.0f)
			return new Vector3 (0.0f, randomHeight, 0.0f);

		// Calculate the positions 
		float xpositions = -(0.5f * ScreenWidthUnits) + ((float)position + 1.0f) * ScreenWidthUnits / denomimator;

		// Return the positions relative to the number of systems
		return new Vector3(xpositions, randomHeight, 0.0f);
	
	}
}
