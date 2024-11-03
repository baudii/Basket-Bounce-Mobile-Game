using UnityEngine;
using KK.Common;
using BasketBounce.Systems;

namespace BasketBounce.Gameplay.Visuals
{
	public class FinishIconHelper : MonoBehaviour
	{
		[SerializeField] GameObject body;

		LevelData currentLevelData;

		private void Awake()
		{
			LevelManager.Instance.OnLevelSetup.AddListener(Setup);
		}

		private void OnDestroy()
		{
			LevelManager.Instance.OnLevelSetup.RemoveListener(Setup);
		}

		private void Update()
		{
			bool isVisible = currentLevelData.IsFinishInScreen();
			if (isVisible && body.activeSelf)
			{
				body.SetActive(false);
			}
			else if (!isVisible && !body.activeSelf)
			{
				body.SetActive(true);
			}
		}

		public void Setup(LevelData levelData)
		{
			transform.position = transform.position.WhereX(levelData.GetFinPos().x);
			currentLevelData = levelData;
		}
	}
}