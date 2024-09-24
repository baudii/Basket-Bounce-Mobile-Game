using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Finish : MonoBehaviour
{
	[SerializeField] float localScaleModifier;
	[SerializeField] float animationSpeed;
	[SerializeField] GameObject netCover;
	[SerializeField] Transform animationStartPoint, animationEndPoint;
	[SerializeField] float secToStart, secToEnd;
	[SerializeField] UnityEvent OnFinish;

	public bool finished;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (finished)
			return;

		if (collision.transform.TryGetComponent(out Ball ball))
		{
			if (ball.CurrentState == Ball.BallState.Finished)
				return;


			finished = true;
			OnFinish?.Invoke();

			StartCoroutine(AnimateFinish(ball));
		}
	}
	IEnumerator AnimateFinish(Ball ball)
	{
		int bounces = ball.OnFinish();
		//подвинуть мяч к точке анимации
		ball.transform.rotation = Quaternion.identity;
		ball.ResetRotation();
		Vector2 startPos = ball.transform.position;
		Vector2 endPos = animationStartPoint.position;
		float t = 0;
		do
		{
			t += Time.deltaTime * animationSpeed;
			ball.transform.position = Vector3.Lerp(startPos, endPos, t);
			yield return null;
		} while (t < 1);
		netCover.SetActive(true);

		//запустить анимацию
		startPos = animationStartPoint.position;
		endPos = animationEndPoint.position;
		t = 0;

		Vector3 initialScale = ball.transform.localScale;

		Vector3 targetScale = ball.transform.localScale.WhereX(initialScale.x * localScaleModifier);
		do
		{
			t += Time.deltaTime * animationSpeed;
			ball.transform.position = Vector3.Lerp(startPos, endPos, t);
			if (t < 0.3f)
				ball.transform.localScale = Vector3.Lerp(initialScale, targetScale, t * (10f / 3f));
			else if (t > 0.6f)
				ball.transform.localScale = Vector3.Lerp(targetScale, initialScale, t * (10f / 3f) - 2);

			yield return null;
		} while (t < 1);

		//ball.transform.localScale = initialScale;
		netCover.SetActive(false);

		//сделать все остальное

		LevelData levelData = LevelManager.Instance.CurrentLevelData;
		int stars = levelData.ConvertToStars(bounces);

		LevelManager.Instance.OnFinish(stars);

		GameManager.Instance.UpdateLevelSelector();
		GameManager.Instance.ShowLevelCompleteScreen(stars);
	}

	/*
		Потенциально переделать в dotween
		public void AnimateFinishTween(Ball ball)
		{
			Sequence sequence = DOTween.Sequence(ball.transform);

			Tween moveToStartPos = transform.DOMove(animationStartPoint.position, secToStart);
			Tween moveToEndPos = transform.DOMove(animationEndPoint.position, secToEnd);
			Tween shrink = transform.DOSc

		}*/
}

