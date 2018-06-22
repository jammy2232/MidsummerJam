using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour 
{

	// Holds all the effects selected tobe placed in the scene
	public List<Effect> CloudEffects;

	// Use this for initialization
	void Start () 
	{

		int counter = 0;

		// Check that all the Effects are valid
		foreach(var effect in CloudEffects)
		{
			if (!effect.ParticleSystem.GetComponent<ParticleSystem> ()) 
			{
				Debug.Log (effect.name + " has NO Particle System");
			} 
			else 
			{
				effect.effectHolder = Instantiate(effect.ParticleSystem);
				effect.effectHolder.transform.position = GetPosition (counter, CloudEffects.Count);
				effect.effectHolder.AddComponent<AudioSource> ();

			}

			counter++;
				
		}

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
					effect.effectHolder.GetComponent<ParticleSystem>().Play();
				}
			}
		}

		// if an input is released check what effects should be deactivated

		foreach (var effect in CloudEffects) 
		{
			if(effect.effectHolder.GetComponent<ParticleSystem>().isPlaying)
			{
				if (!Input.GetKey (effect.KeyToActivate)) 
				{
					effect.effectHolder.GetComponent<ParticleSystem>().Stop ();
				}
			}

		}

	}


	// Function to return the position of a particle system based the number of systems and it's place in the list
	Vector3 GetPosition(int position, int totalNumberofSystems)
	{

		// Get the main Camerea W value
		float ScreenHeightUnits = Camera.main.orthographicSize * 2.0f;
		float ScreenWidthUnits = ScreenHeightUnits * Screen.width / Screen.height;

		// Catch for one system
		if (totalNumberofSystems == 1)
			return new Vector3 (0.0f, -5.0f, 0.0f);

		// the y and z positions are fixed as the visuals are 3d 
		float denomimator = totalNumberofSystems + 1;

		if (denomimator == 0.0f)
			return new Vector3 (0.0f, 0.0f, 0.0f);

		// Calculate the positions 
		float xpositions = -(0.5f * ScreenWidthUnits) + ((float)position + 1.0f) * ScreenWidthUnits / denomimator;

		// Return the positions relative to the number of systems
		return new Vector3(xpositions, -5.0f, 0.0f);
	
	}


}
