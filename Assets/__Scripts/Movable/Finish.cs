using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
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
		int stars = ball.OnFinish();
		//подвинуть м€ч к точке анимации
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

		//LevelData levelData = LevelManager.Instance.CurrentLevelData;
		//ScoreData scoreData = levelData.ConvertToStars(bounces);

		LevelManager.Instance.OnFinish(stars);

		GameManager.Instance.ShowLevelCompleteScreen(stars);
	}

	/*
		ѕотенциально переделать в dotween
		public void AnimateFinishTween(Ball ball)
		{
			Sequence sequence = DOTween.Sequence(ball.transform);

			Tween moveToStartPos = transform.DOMove(animationStartPoint.position, secToStart);
			Tween moveToEndPos = transform.DOMove(animationEndPoint.position, secToEnd);
			Tween shrink = transform.DOSc

		}*/
}

public struct ScoreData
{
	public int stars;
	public int nextStarBounceRequirement;

	public ScoreData(int stars, int nextStarBounceRequirement)
	{
		this.stars = stars;
		this.nextStarBounceRequirement = nextStarBounceRequirement;
	}
	/* јвось пригодитьс€ ?
   // «наки сравнени€ перевернуты, потому что формула расчета score тем больше, чем больше времени и баунсов.
   // ј в игре надо минимизировать показатели. ћожно было развернуть дробь, но так проще и результат тот же
   public static bool operator <=(ScoreData left, ScoreData right)
   {
	   return left.Score >= right.Score;
   }
   public static bool operator >=(ScoreData left, ScoreData right)
   {
	   return left.Score <= right.Score;
   }
   public static bool operator <(ScoreData left, ScoreData right)
   {
	   return left.Score > right.Score;
   }
   public static bool operator >(ScoreData left, ScoreData right)
   {
	   return left.Score < right.Score;
   }
   public static bool operator ==(ScoreData left, ScoreData right)
   {
	   return Equals(left, right);
   }	
   public static bool operator !=(ScoreData left, ScoreData right)
   {
	   return left.bounces != right.bounces || left.totalTime != right.totalTime;
   }

   public override bool Equals(object obj)
   {
	   if (obj is ScoreData score)
	   {
		   return totalTime == score.totalTime && bounces == score.bounces;
	   }
	   return false;
   }

   public override int GetHashCode()
   {
	   return HashCode.Combine(bounces, totalTime);
   }*/
}

