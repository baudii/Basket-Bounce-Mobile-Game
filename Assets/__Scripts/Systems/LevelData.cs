using UnityEngine;
using UnityEngine.Events;

public class LevelData : MonoBehaviour
{
	[SerializeField] Finish fin;
	[SerializeField] ResetableManager resetableManager;

	[SerializeField] int bounces3star;
	[SerializeField] int bounces2star;
	[SerializeField] bool useThreshHold;
	[SerializeField] bool canNotStuck;
	[SerializeField] float overrideStuckTime;
	[SerializeField] GameObject tutorialToDisable;
	[SerializeField] GameObject[] disableOnClickAnywhere;
	[SerializeField] UnityEvent OnFirstTimeLoadEvemt;
/*	[SerializeField] float time3star;
	[SerializeField] float time2star;*/

	public bool UseThreshold => useThreshHold;
	public bool CanNotStuck => canNotStuck;
	public float OverrideStuckTime => overrideStuckTime;

#if UNITY_EDITOR
	[Header("Editor Only")]
	[SerializeField] bool validateShadows;
	[SerializeField] bool shadowsEnabled;
	private void OnValidate()
	{
		if (validateShadows)
		{
			AdjustShadows(shadowsEnabled);
		}
	}

	public void AdjustShadows(bool enable = true)
	{
		transform.ForEachDescendant(child =>
		{
			if (child.TryGetComponent(out GFX_Shadow shadow))
			{
				shadow.Adjust();
				shadow.gameObject.SetActive(enable);
			}
		});
	}
#endif

	public void Init()
	{
		ResetLevel();
		if (tutorialToDisable != null)
			tutorialToDisable.SetActive(false);
		foreach (var item in disableOnClickAnywhere)
		{
			if (item != null)
				item.SetActive(false);
		}

	}

	public void OnFirstTimeLoad()
	{
		if (tutorialToDisable != null)
			tutorialToDisable.SetActive(true);
		foreach (var item in disableOnClickAnywhere)
		{
			if (item != null)
				item.SetActive(true);
		}
		OnFirstTimeLoadEvemt?.Invoke();
	}

	public void OnBallRealeased()
	{
		if (tutorialToDisable != null)
			tutorialToDisable.SetActive(false);
	}

	public void OnClickAnywhere()
	{
		foreach(var item in disableOnClickAnywhere)
		{
			if (item != null)
				item.SetActive(false);
		}
	}

	public void OnLevelUnload()
	{
		ResetLevel();
	}

	public void ResetLevel()
	{
		fin.finished = false;
		if (resetableManager != null)
			resetableManager.ResetAll();
	}

	public Vector3 GetFinPos()
	{
		return fin.transform.position;
	}

	public ScoreData ConvertToStars(int bounces)
	{
		if (bounces2star == 0)
			bounces2star = bounces3star + 3;
		if (bounces <= bounces3star)
			return new ScoreData(3, 0);
		if (bounces <= bounces2star)
			return new ScoreData(2, bounces3star);
		return new ScoreData(1, bounces2star);
	}
}
