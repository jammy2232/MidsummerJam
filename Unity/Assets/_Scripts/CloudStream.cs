﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class CloudStream : MonoBehaviour 
{

	public ParticleSystem particles;
	public AudioSource musicSource;
	public AudioSource soundSource;
	public AudioMixerSnapshot snapshot;
	private bool volumeDown = true;

	public void Init()
	{

		// Prepare all the references to work with the cloud streams
		particles = GetComponent<ParticleSystem>();

		// Get the reference to the audio sources 
		AudioSource[] sources = GetComponents<AudioSource>();
		volumeDown = true;
		musicSource = sources [0];
		soundSource = sources [1];

	}

		
	public void Play()
	{
		
		if (!musicSource.isPlaying)
		{
			volumeDown = false;
			musicSource.Play ();
			soundSource.Play();
			snapshot.TransitionTo (10.0f);
		}
			
	}


	public void Stop()
	{
		volumeDown = true;
	}


	// Update is called once per frame
	void Update() 
	{

		if (volumeDown) {
			musicSource.volume = Mathf.Clamp (musicSource.volume - 0.1f * Time.deltaTime, 0.0f, 1.0f);

			if (musicSource.volume < 0.00001f) {
				musicSource.Stop ();
			}

		} else 
		{
			musicSource.volume = Mathf.Clamp (musicSource.volume + 0.005f * Time.deltaTime, 0.0f, 1.0f);
		}
			
	}


}
