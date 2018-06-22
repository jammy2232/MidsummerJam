using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CloudEffect", menuName = "CloudEffect")]
public class Effect : ScriptableObject {

	// This is a container to generate different types of effects 
	public ParticleSystem ParticleSystem;

	// The key with which to action this system\
	[Tooltip("This should correspond to one of the Makey Makey Key inputs")]
	public KeyCode KeyToActivate;

}
