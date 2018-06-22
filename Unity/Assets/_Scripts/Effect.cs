using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CloudEffect", menuName = "CloudEffect")]
public class Effect : ScriptableObject {

	// This is a container to generate different types of effects 
	public GameObject ParticleSystem;

	// The key with which to action this system\
	[Tooltip("This should correspond to one of the Makey Makey Key inputs")]
	public KeyCode KeyToActivate;

	// References to the music item this effect will play
	[Header("Audio Effects")]
	public AudioClip musicClip;
	[Range(-1.0f, 0.0f)]
	public float pitch;
	[Range(-1.0f, 0.0f)]
	public float tempo;
	[Range(0.0f, 0.0f)]
	public float fadeIn;
	[Range(0.0f, 0.0f)]
	public float fadeOut;

	[HideInInspector]
	public GameObject effectHolder;


}
