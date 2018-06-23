using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Note this has not been designed in a normal Unity friendly way
public class GlobalWind 
{
	// Anything heigher than the windHeight is affected by wind
	public float windHeight = 10.5f;
	// Magnitude and direction of the wind
	public float windDirection = 1.0f;
	// Requires access to the CloudStream to make wind blow the particles
	public CloudStream[] objects;

	// Constructor
	public GlobalWind(CloudStream[] _objects)
	{
		objects = _objects;
	}
		
	// Update is called once per frame
	public void Update () 
	{
		// Access all of the particles that are currently in play for each stream
		// and give them some wind if above the predefined height
		foreach (CloudStream obj in objects)
		{		
			ParticleSystem.Particle[] streamsParticles = new ParticleSystem.Particle[obj.particles.main.maxParticles];

			// GetParticles is allocation free because we reuse the streamsParticles buffer between updates
			int numParticlesAlive = obj.particles.GetParticles(streamsParticles);

			// Change only the particles that are alive
			for(var i = 0; i < streamsParticles.Length; ++i)
			{
				if (streamsParticles[i].position.z > windHeight)
				{
					// We add/remove more x velocity over the lifetime of the particle
					// this makes it a bit more realistic compared to sticking to the
					// one x velocity. Remember also there can be x, y, z velocity on the particles
					// from the particle generator
					streamsParticles[i].velocity += new Vector3(Mathf.Clamp(windDirection, -1.0f, 1.0f), 0.0f, 0.0f) * 0.5f;
				}
			}

			// Apply the particle changes to the particle system
			obj.particles.SetParticles(streamsParticles, numParticlesAlive);
		}
	}
}
