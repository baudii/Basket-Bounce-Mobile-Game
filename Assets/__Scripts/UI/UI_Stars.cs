using DG.Tweening;
using UnityEngine;
using KK.Common;

namespace BasketBounce.UI
{
	public class UI_Stars : MonoBehaviour
	{
		[SerializeField] UI_StarAnimation[] starAnimations;
		[SerializeField] float animDuration;
		[SerializeField] float delayBetweenAnimations;
		[SerializeField] Ease ease;
		[SerializeField] GameObject screenBlocker;
		[SerializeField] AudioSource src;

		Sequence seq;

		public void SetStars(int stars)
		{

			foreach (var anim in starAnimations)
			{
				anim.ResetState();
			}
			screenBlocker.SetActive(true);
			int n = 0;
			if (seq != null)
				seq.Kill();
			seq = DOTween.Sequence(transform);
			for (int i = 0; i < stars; i++)
			{
				float delay = i * animDuration / 3f;

				Tween[] tweens = starAnimations[i].GetTweens(animDuration, ease);
				foreach (var tween in tweens)
				{
					seq.Insert(delay, tween);
				}
				seq.InsertCallback(delay + animDuration * 0.8f, () =>
				{
					src.pitch = 1 + 3f * 0.1f * n;
					src.PlayOneShot(src.clip);
					n++;
				});
			}
			seq.SetUpdate(true).SetAutoKill(false);
			seq.OnComplete(() => screenBlocker.SetActive(false));
			seq.Play();
		}

		public void KillTween()
		{
			DOTween.Kill(transform, true);
			screenBlocker.SetActive(false);
		}
	}
}