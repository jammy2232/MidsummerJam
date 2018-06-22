using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudStream : MonoBehaviour 
{

	public ParticleSystem particles;
	public AudioSource musicSource;
	public AudioSource soundSource;

	public void Init()
	{

		// Prepare all the references to work with the cloud streams
		particles = GetComponent<ParticleSystem>();

		// Get the reference to the audio sources 
		AudioSource[] sources = GetComponents<AudioSource>();
		musicSource = sources [0];
		soundSource = sources [1];

	}

		
	public void Play()
	{
		
		if (!particles.isPlaying)
		{
			particles.Play ();
			musicSource.Play ();
			soundSource.Play();
		}

	}


	public void Stop()
	{

		if (particles.isPlaying)
		{
			particles.Stop ();
			musicSource.Stop ();
			soundSource.Stop ();
		}

	}


	// Update is called once per frame
	void Update () 
	{
		
	}


}
