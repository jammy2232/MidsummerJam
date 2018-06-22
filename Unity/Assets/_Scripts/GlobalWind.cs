using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalWind {

	public GlobalWind(CloudStream[] _objects)
	{
		objects = _objects;
	}

	public float windHeight = 10.5f;
	static public CloudStream[] objects;
	// Update is called once per frame
	public void Update () {
		// Blow some wind at a predefined height
		foreach (CloudStream obj in objects)
		{
			ParticleSystem.Particle[] streamsParticles = new ParticleSystem.Particle[obj.particles.main.maxParticles];
			// GetParticles is allocation free because we reuse the streamsParticles buffer between updates
			int numParticlesAlive = obj.particles.GetParticles(streamsParticles);

			// Change only the particles that are alive
			for (int i = 0; i < numParticlesAlive; i++)
			{
				if (streamsParticles[i].position.z > windHeight)
				{
					streamsParticles[i].velocity += Vector3.right * 0.5f;
				}
			}

			// Apply the particle changes to the particle system
			obj.particles.SetParticles(streamsParticles, numParticlesAlive);
		}
	}
}
