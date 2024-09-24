using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class UI_TweenFade : MonoBehaviour
{
	[SerializeField] MaskableGraphic target;
	[SerializeField, Range(0,5)] float fadeTime, unfadeTime;

	private void OnDisable()
	{
		DOTween.Kill(target);
		var col = target.color;
		col.a = 0;
		target.color = col;
	}

	public TweenerCore<Color, Color, ColorOptions> Fade()
	{
		DOTween.Kill(target);
		return target.DOFade(0, fadeTime);
	}

	public TweenerCore<Color, Color, ColorOptions> Unfade()
	{
		DOTween.Kill(target);
		return target.DOFade(1, unfadeTime);
	}
}
