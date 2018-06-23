using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(fileName = "CloudEffect", menuName = "CloudEffect")]
public class Effect : ScriptableObject {

	// This is a container to generate different types of effects 
	[Tooltip("The prefab containing the particle effect to be rendered")]
	public GameObject ParticleSystem;

	// References to the music item this effect will play
	[Header("Audio Effects")]
	public AudioMixerGroup musicMixer;
	public AudioMixerSnapshot RequiredSnapShot;
	// Looping music clip that fades in and out on button press
	public AudioClip musicClip;
	public AudioMixerGroup soundMixer;
	// Non looping sound clip that plays once on button press
	public AudioClip soundsClip;
	// For music clips
	public float FadeInRate;
	public float FadeOutRate;

	// Spawn location is where particles will generate at.
	public CloudStream SpawnEffect(Vector3 SpawnLocation)
	{
		// Create the particle system that generates our cloud particles
		GameObject newEffect = Instantiate(ParticleSystem);

		// Check the effect has a Particle System
		if (!newEffect.GetComponent<ParticleSystem>()) 
		{
			// This should never happen but its best to be safe.
			Debug.Log (newEffect.name + " has NO Particle System");
			return null;
		} 

		// Position where the particle generator will spawn particles from 
		newEffect.transform.position = SpawnLocation;

		/// Audio Parameters
		// 2 components for Music (looping with fadeing) and Sound (non-looping no fade)
		newEffect.AddComponent<AudioSource>();
		newEffect.AddComponent<AudioSource>();

		// get references to edit these sources 
		AudioSource[] sources = newEffect.GetComponents<AudioSource>();

		// Setup the music
		sources[0].clip = musicClip;
		sources[0].loop = true;
		sources[0].outputAudioMixerGroup = musicMixer;

		// Setup the sound
		sources[1].clip = soundsClip;
		sources[1].loop = false;
		sources[1].outputAudioMixerGroup = soundMixer;

		// Add a CloudStreamComponent and set it up
		newEffect.AddComponent<CloudStream>();
		newEffect.GetComponent<CloudStream>().snapshot = RequiredSnapShot;
		newEffect.GetComponent<CloudStream>().Init();

		return newEffect.GetComponent<CloudStream>();
	}
}
