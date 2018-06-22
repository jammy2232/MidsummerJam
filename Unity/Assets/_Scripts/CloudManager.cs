using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudManager : MonoBehaviour 
{

	// Holds all the effects selected tobe placed in the scene
	public List<Effect> CloudEffects;

	public KeyCode[] keys =
	{
		KeyCode.W,
		KeyCode.E,
		KeyCode.R,
		KeyCode.T,
		KeyCode.Y,
		KeyCode.U,
		KeyCode.I,
		KeyCode.O,
	};

	private CloudStream[] objects;

	// Use this for initialization
	void Start () 
	{
		// Create a array to hold the number of possible effects
		objects = new CloudStream[CloudEffects.Count]; 

		// temp counter
		int counter = 0;

		// Check that all the Effects are valid
		foreach(var effect in CloudEffects)
		{
			objects [counter] = effect.SpawnEffect (GetPosition (counter, CloudEffects.Count));
			counter++;
		}

	}

	public float windHeight = 10.5f;
	// Update is called once per frame
	void Update () 
	{

		// if an input is fired check what effects should be activated
		if (Input.anyKeyDown) 
		{

			for(int i = 0; i < objects.Length; ++i)
			{
				if(Input.GetKeyDown(keys[i]))
				{
					objects [i].Stop();
				}
			}

		}

		// if an input is released check what effects should be deactivated

		for(int i = 0; i < objects.Length; ++i)
		{
			if(objects[i].GetComponent<ParticleSystem>().isStopped)
			{
				if (!Input.GetKey(keys[i])) 
				{
					objects [i].Play();
				}
			}
		}

		foreach (CloudStream obj in objects)
		{
			ParticleSystem.Particle[] m_Particles = new ParticleSystem.Particle[obj.particles.main.maxParticles];
			// GetParticles is allocation free because we reuse the m_Particles buffer between updates
			int numParticlesAlive = obj.particles.GetParticles(m_Particles);

			//windHeight += 0.005f;
			//if (windHeight > 10.0f)
			//{
			//	windHeight = 2.5f;
			//}

			// Change only the particles that are alive
			for (int i = 0; i < numParticlesAlive; i++)
			{
				if (m_Particles[i].position.z > windHeight)
				{
					m_Particles[i].velocity += Vector3.right * 0.5f;
				}
			}

			// Apply the particle changes to the particle system
			obj.particles.SetParticles(m_Particles, numParticlesAlive);
		}
	}


	// Function to return the position of a particle system based the number of systems and it's place in the list
	Vector3 GetStartParticlePosition(int position, int totalNumberofSystems)
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
