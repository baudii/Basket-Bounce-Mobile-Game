using DG.Tweening;
using TMPro;
using UnityEngine;

public class UI_BounceCounter : MonoBehaviour
{
	[SerializeField] TextMeshProUGUI textObject;
	[SerializeField] UI_TweenScale tweenScale;
	[SerializeField] UI_TweenFade tweenFade;

	public void OnBounce(int bounces)
	{
		textObject.text = bounces.ToString();
		tweenScale.TweenToFrom();
		tweenFade.Unfade().OnComplete(() => tweenFade.Fade());
	}

	public void ResetState()
	{
		textObject.text = "0";
	}
}
