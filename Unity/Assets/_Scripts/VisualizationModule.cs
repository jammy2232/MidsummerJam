using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(menuName = "Visualisation Module")]
public class VisualizationModule : ScriptableObject 
{

	[Header("Visulaisation settings")]
	public float baseSize = 5.0f;
	public float sizeMultiplier = 60.0f;
	public float baseSpeed = 5.0f;
	public float speedMultiplier = 60.0f;
	[Range(0, 5)]
	public int speedFrequencyBand = 0;
	public float rateMultiplier = 50.0f;
	public FFTWindow windowType = FFTWindow.Rectangular;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void ApplyToParticleSystem (ParticleSystem system) 
	{
		
	}



}
