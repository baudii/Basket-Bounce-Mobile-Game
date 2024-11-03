using System.Collections;
using UnityEngine;

namespace KK.Common
{
	public class AudioCrossFade : Singleton<AudioCrossFade>
	{
		[SerializeField] AudioSourceInfo[] audioSources;
		[SerializeField] float transitionSpeed;
		int i = 0;
		AudioSourceInfo currentSource;
		bool toBreak;

		protected override void OnAwake()
		{
			if (audioSources.Length == 0)
				return;

			currentSource = audioSources[0];
			currentSource.src.volume = currentSource.maxVolume;
			currentSource.src.Play();
		}

		public void SetExactSourse(int index)
		{
			if (i == index)
				return;

			i = index;
			toBreak = true;
			StartCoroutine(SwitchSourceTo(audioSources[index]));
		}

		[ContextMenu("Next")]
		public void Next()
		{
			i = (i + 1) % audioSources.Length;
			toBreak = true;
			StartCoroutine(SwitchSourceTo(audioSources[i]));
		}

		IEnumerator SwitchSourceTo(AudioSourceInfo newSource)
		{
			float t = 0;
			yield return null;
			float currentLerpStart = currentSource.src.volume;
			toBreak = false;
			newSource.src.Play();
			newSource.src.volume = 0;

			while (true)
			{
				currentSource.src.volume = Mathf.Lerp(currentLerpStart, 0, t);
				newSource.src.volume = Mathf.Lerp(0, newSource.maxVolume, t);

				if (t >= 1 || toBreak)
					break;

				t += Time.deltaTime * transitionSpeed;
				yield return null;
			}
			toBreak = false;
			currentSource.src.Stop();
			currentSource = newSource;
		}

		[System.Serializable]
		class AudioSourceInfo
		{
			public AudioSource src;
			[Range(0, 1f)] public float maxVolume;
		}
	}
}