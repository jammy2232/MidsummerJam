using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour 
{

	// Holds all the effects selected to be placed in the scene
	public List<Effect> CloudEffects;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{

		// if an input is fired check what effects should be activated
		if (Input.anyKeyDown) 
		{

			foreach(var effect in CloudEffects)
			{
				if(Input.GetKeyDown(effect.KeyToActivate))
				{
					effect.ParticleSystem.Play ();
				}

			}
		}

		// if an input is released check what effects should be deactivated
		foreach (var effect in CloudEffects) 
		{
			if(effect.ParticleSystem.isPlaying)
			{
				if (!Input.GetKeyDown (effect.KeyToActivate)) 
				{
					effect.ParticleSystem.Stop ();
				}
			}

		}

	}
}
