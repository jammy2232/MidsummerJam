using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Visualisation Module")]
public class VisualizationModule : ScriptableObject 
{

	// Public interface for tweaking the visualisation 
	[Header("Visulaisation settings")]
	public float baseSize = 5.0f;
	public float sizeMultiplier = 60.0f;
	public float baseSpeed = 5.0f;
	public float speedMultiplier = 60.0f;
	[Range(0, 5)]
	public int speedFrequencyBand = 0;
	public float rateMultiplier = 50.0f;
	public FFTWindow windowType = FFTWindow.Rectangular;

	[Header("Frequency Analysis Settings")]
	// We perform fourier transforms on the audio to get the frequency of the audio.
	// This is then used to change the colour of particles. To make the colour changes
	// more gradual you can increase the number of buckets. This must be a multiple of 2
	// see AudioListener.GetSpectrumData for all of the constraints
	public const int FrequencyBuckets = 1024;

	// Place to store the specturum for Frequency Data Analysis
	private float[] frequencySpectrum;
	private float[] timeDomationData;

	// Split frequencies into 3 segments for RGB of colour and get the greatest component frequency
	private int[] largestFrequencyBucket     = { 0,    0,    0,		0, 		0, 		0 };
	private float[] magnitudeOfLargestBucket = { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };

	// A number of persistant characteristics 
	float total = 0.0f;
	float totalMag = 0.0f;
	float completeTotalMag = 0.0f;

	// Update is called once per frame
	public void ApplyToParticleSystem (ParticleSystem system) 
	{

		// Create a new particle colour based on the frequency in the stored information
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

		// Apply same base effects to the particle system
		var particleEmitter	       = system.main;
		particleEmitter.startSize  =  baseSize + completeTotalMag * sizeMultiplier;
		particleEmitter.startSpeed = baseSpeed + magnitudeOfLargestBucket[speedFrequencyBand] * speedMultiplier;
		particleEmitter.startColor = newColour;

		// Effect the rate of emmission
		var particleEmissionSystem = system.emission;
		particleEmissionSystem.rateOverTime = completeTotalMag * rateMultiplier;
		
	}

	private void PerformFrequencyAnalysis(int[] largestFrequencyBucket, float[] magnitudeOfLargestBucket)
	{

		// Get the Spectrum data
		AudioListener.GetSpectrumData(frequencySpectrum, 0, windowType);

		// Get the Time Domain Data (Is this Channel Correct??)
		AudioListener.GetOutputData(timeDomationData, 0);

		// Go through each of the frequecy subsections and determine some characteristics
		for (int j = 0; j < 6; ++j)
		{
			for (int i = j * FrequencyBuckets / 6; i < (j + 1) * FrequencyBuckets / 6; ++i)
			{
				if (magnitudeOfLargestBucket[j] < frequencySpectrum[i])
				{
					// Find the where the largest spike occurs in the specific domain
					largestFrequencyBucket[j] = i - j * FrequencyBuckets / 6;
					// What is the spike magnitude
					magnitudeOfLargestBucket[j] = frequencySpectrum[i];
				}
			}
		}

		// Normalising factor to emphasise change (+0.00001 to prevent divide by zero)
		total = largestFrequencyBucket[0] + largestFrequencyBucket[1] + largestFrequencyBucket[2] + 0.000001f;

		// Normalising factor to emphasise change on the magnitudes
		totalMag = magnitudeOfLargestBucket[0] + magnitudeOfLargestBucket[1] + magnitudeOfLargestBucket[2];
		completeTotalMag = totalMag + magnitudeOfLargestBucket [3] + magnitudeOfLargestBucket [4] + magnitudeOfLargestBucket [5];
	
	}

}
