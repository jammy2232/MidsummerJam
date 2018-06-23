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
	public AudioClip musicClip;
	public AudioMixerGroup soundMixer;
	public AudioClip soundsClip;
	public float FadeInRate;
	public float FadeOutRate;

	// Instanciate the effect setup to work
	public CloudStream SpawnEffect(Vector3 SpawnLocation)
	{
		// Create the Object
		GameObject newEffect = Instantiate(ParticleSystem);

		// Check the effect has a Particle System otherwise return Null and log the error
		if (!newEffect.GetComponent<ParticleSystem> ()) 
		{
			Debug.Log (newEffect.name + " has NO Particle System");
			return null;
		} 

		// Position it 
		newEffect.transform.position = SpawnLocation;

		// Add and assign the Audio Parameters 

		// Add the components for Music and Sound Music ref 0 and Sound ref 1
		newEffect.AddComponent<AudioSource>();
		newEffect.AddComponent<AudioSource>();

		// get references to edit these sources as per the object paramaters 
		AudioSource[] sources = newEffect.GetComponents<AudioSource>();

		// Setup the music 
		sources[0].clip = musicClip;
		sources[0].outputAudioMixerGroup = musicMixer;

		// Setup the audio 
		sources[1].clip = soundsClip;
		sources [1].loop = false;
		sources[1].outputAudioMixerGroup = soundMixer;

		// Add a CloudSteamComponent and set it up
		newEffect.AddComponent<CloudStream>();
		newEffect.GetComponent<CloudStream>().snapshot = RequiredSnapShot;
		newEffect.GetComponent<CloudStream>().Init();

		// Return the gameObject Reference 
		return newEffect.GetComponent<CloudStream>();

	}
		
}
