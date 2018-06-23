using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CloudStream : MonoBehaviour 
{
	public ParticleSystem particles;

	// Looping audio that plays whilst button pressed
	// Sound fades in and out
	public AudioSource musicSource;
	// Instantaneous audio that will play immediately when button pressed
	public AudioSource soundSource;

	public AudioMixerSnapshot snapshot;
	private bool volumeDown = true;

	public void Init()
	{
		// Prepare all the references to work with the cloud streams
		particles = GetComponent<ParticleSystem>();

		// Get the reference to the audio sources 
		AudioSource[] sources = GetComponents<AudioSource>();
		musicSource = sources[0];
		soundSource = sources[1];
	}

		
	public void Play()
	{
		if (!musicSource.isPlaying)
		{
			// The volume starts at 0 and fades in each frame
			volumeDown = false;
			musicSource.volume = 0.0f;

			musicSource.Play();
			soundSource.Play();

			// To make multiple audio sources work together we transition the mixer parameters
			// over a number of seconds
			snapshot.TransitionTo(10.0f);
		}
	}


	public void Stop()
	{
		// Music is not stopped directly we fade the sound out by decreasing the volume
		// before stopping the sound.
		volumeDown = true;
	}


	// Update is called once per frame
	void Update() 
	{
		// As above sounds are faded in and out when starting/stopping
		if (volumeDown)
		{
			// Volume in Unity is a float between 0 and 1
			musicSource.volume = Mathf.Clamp (musicSource.volume - 0.1f * Time.deltaTime, 0.0f, 1.0f);

			// Remember James floats are not precise so don't compare to 0.0f
			// When volume is super low it is now safe to stop the sound
			if (musicSource.volume < 0.00001f)
			{
				musicSource.Stop();
			}
		}
		else
		{
			musicSource.volume = Mathf.Clamp (musicSource.volume + 0.005f * Time.deltaTime, 0.0f, 1.0f);
		}
	}
}
