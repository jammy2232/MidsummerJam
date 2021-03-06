﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CloudManager : MonoBehaviour 
{
	[Header("Cloud Types and Controls")]
	// Holds all the effects selected to be placed in the scene
	public List<Effect> CloudEffects;

	// Adjust the height of the particles effects (This is applied to all particle systems)
	public float HeightAdjust = 0.0f;

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

	[Header("Wind Settings")]
	// Bool to determine if to add wind effects
	public bool addWind = false;
	// Anything above this height will be affected by wind.
	public float windHeight = 2.5f;
	// This also controls how powerful the wind is
	public float windDirection = -1.0f;

	// This is a timer to reset the game after 10 minutes of no input 
	private float timer = 0.0f;

	// We perform fourier transforms on the audio to get the frequency of the audio.
	// This is then used to change the colour of particles. To make the colour changes
	// more gradual you can increase the number of buckets. This must be a multiple of 2
	// see AudioListener.GetSpectrumData for all of the constraints
	[Header("Frequency Analysis Settings")]
	public const int FrequencyBuckets = 1024;

	[Header("Visulaisation settings")]
	public float baseSize = 5.0f;
	public float sizeMultiplier = 60.0f;
	public float baseSpeed = 5.0f;
	public float speedMultiplier = 60.0f;
	[Range(0, 5)]
	public int speedFrequencyBand = 0;
	public float rateMultiplier = 50.0f;

	// Place to store the specturum for Frequency Data Analysis
	private float[] spectrum;

	// Pribate Variables to hold reference to object list
	private CloudStream[] objects;

	// Wind application object
	private GlobalWind windEffect;

	// Temp fix
	int currentSong = 0;
	public float timerinput = 0.0f;

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

		// Activate the first song to start off the experience
		objects[0].Play();

	}
	
	// Update is called once per frame
	void Update()
	{

		// update the timer
		timer += Time.deltaTime;
		timerinput += Time.deltaTime;

		// Hard limit of 10 minutes (mins * seconds)
		if (timer > (10.0f * 60.0f))
		{
			SceneManager.LoadScene ("MainMenu", LoadSceneMode.Single);
		}

		// if an input is fired check what effects should be activated
		if (Input.anyKeyDown)
		{

			// hardcode Fix
			if (Input.GetKeyDown (KeyCode.D) && Input.GetKeyDown (KeyCode.G) && Input.GetKeyDown (KeyCode.F) && timerinput > 0.3f) {


				currentSong -= 1;

				timerinput = 0.0f;

			} else
			{

				currentSong += 1;

				timerinput = 0.0f;


			}

			if (currentSong > 5) {
				currentSong = 0;
			} else if (currentSong < 0) {
				currentSong = 5;
			}


			for (int i = 0; i < objects.Length; ++i)
			{
				if (i == currentSong) {
					objects [currentSong].Play ();
				} else {
					objects [i].Stop ();
				}
			}


		}

		// Escape the Game
		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			SceneManager.LoadScene ("MainMenu", LoadSceneMode.Single);
		}

		// if an input is released check what effects should be deactivated
//		for (int i = 0; i < objects.Length; ++i)
//		{
//			if (objects[i].musicSource.isPlaying)
//			{
//				if (!Input.GetKey(keys[i]))
//				{
//					objects[i].Stop();
//				}
//			}
//		}

		// Split frequencies into 3 segments for RGB of colour and get the greatest component frequency
		int[] largestFrequencyBucket     = { 0,    0,    0,		0, 		0, 		0 };
		float[] magnitudeOfLargestBucket = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };
		PerformFrequencyAnalysis(largestFrequencyBucket, magnitudeOfLargestBucket);

		// Normalising factor to emphasise change (+0.00001 to prevent divide by zero)
		float total = largestFrequencyBucket[0] + largestFrequencyBucket[1] + largestFrequencyBucket[2] + 0.000001f;

		// Normalising factor to emphasise change on the magnitudes
		float totalMag = magnitudeOfLargestBucket[0] + magnitudeOfLargestBucket[1] + magnitudeOfLargestBucket[2];
		float completeTotalMag = totalMag + magnitudeOfLargestBucket [3] + magnitudeOfLargestBucket [4] + magnitudeOfLargestBucket [5];

		// Debug.Log (completeTotalMag);

		// Create a new particle colour based on the frequency
		Color newColour;
		if(completeTotalMag < 0.00001f)
		{
			newColour = Color.black;
		}
		else
		{
			newColour = new Color(
				largestFrequencyBucket[0] / total,
				largestFrequencyBucket[1] / total,
				largestFrequencyBucket[2] / total,
				1.0f);
		}

		foreach (var obj in objects)
		{
			var particleEmitter	       = obj.particles.main;
			// particleEmitter.startSize  =  5.0f + magnitudeOfLargestBucket[0] * 50.0f;
			// particleEmitter.startSpeed = 3.0f + magnitudeOfLargestBucket[1] * 15.0f;
			particleEmitter.startSize  =  baseSize + completeTotalMag * sizeMultiplier;
			particleEmitter.startSpeed = baseSpeed + magnitudeOfLargestBucket[speedFrequencyBand] * speedMultiplier;
			particleEmitter.startColor = newColour;


			var particleEmissionSystem = obj.particles.emission;
			particleEmissionSystem.rateOverTime = completeTotalMag * rateMultiplier;

		}

		if (addWind)
		{
			windEffect.windDirection = (magnitudeOfLargestBucket [0] - magnitudeOfLargestBucket [1]) / totalMag;
			windEffect.windHeight = windHeight;
			windEffect.Update ();
		}

//		// Update the camera background colour
//		Camera.main.backgroundColor = new Color(
//			Mathf.Clamp((largestFrequencyBucket[2]),0.0f, 0.1f),
//			Mathf.Clamp((largestFrequencyBucket[3]),0.0f, 0.1f),
//			Mathf.Clamp((largestFrequencyBucket[4]),0.0f, 0.1f),
//			1.0f);


	}

	void PerformFrequencyAnalysis(int[] largestFrequencyBucket, float[] magnitudeOfLargestBucket)
	{
		AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Triangle);

		for (int j = 0; j < 6; ++j)
		{
			for (int i = j * FrequencyBuckets / 6; i < (j + 1) * FrequencyBuckets / 6; ++i)
			{
				if (magnitudeOfLargestBucket[j] < spectrum[i])
				{
					largestFrequencyBucket[j] = i - j * FrequencyBuckets / 6;
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
			return new Vector3 (0.0f, randomHeight, 10.0f);

		// Calculate the positions 
		float xpositions = -(0.5f * ScreenWidthUnits) + ((float)position + 1.0f) * ScreenWidthUnits / denomimator;

		// Return the positions relative to the number of systems
		return new Vector3(xpositions, HeightAdjust + randomHeight, 10.0f);
	
	}
}
