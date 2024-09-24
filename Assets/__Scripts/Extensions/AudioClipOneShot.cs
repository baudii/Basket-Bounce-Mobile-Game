using System;
using UnityEngine;

[Serializable]
public struct AudioClipOneShot
{
	public AudioClip clip;
	[Range(0, 1)] public float volume;
	[Range(0, 1.5f)] public float randomizer;
}
