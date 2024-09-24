using UnityEngine;

public class LevelData : MonoBehaviour
{
	[SerializeField] Finish fin;
	[SerializeField] ResetableManager resetableManager;

	[SerializeField] int bounces3star;
	[SerializeField] int bounces2star;

#if UNITY_EDITOR
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
		transform.ForAllDescendants(child =>
		{
			if (child.TryGetComponent(out GFX_Shadow shadow))
			{
				shadow.Adjust();
				shadow.gameObject.SetActive(enable);
			}
		});
	}
#endif

	public void OnLevelLoad()
	{

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

	public int ConvertToStars(int bounces)
	{
		if (bounces <= bounces3star)
			return 3;
		if (bounces <= bounces2star)
			return 2;
		return 1;
	}
}
