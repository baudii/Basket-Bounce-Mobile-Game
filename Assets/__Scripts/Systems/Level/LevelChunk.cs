using KK.Common;
using System.Collections.Generic;
using UnityEngine;

namespace BasketBounce.Systems
{
    public class LevelChunk : MonoBehaviour
    {
		[SerializeField] List<LevelData> levels;
		public int LevelCount => levels.Count;

#if UNITY_EDITOR

		[SerializeField] bool validate;
		private void OnValidate()
		{
			if (validate)
			{
				ValidateChunk();
				validate = false;
			}
		}

#endif

		public void ValidateChunk()
		{
			levels = new List<LevelData>();

			int i = 1;
			foreach (Transform child in transform)
			{
				if (child.TryGetComponent(out LevelData levelData))
				{
					levelData.gameObject.name = "Level " + i;
					levels.Add(levelData);
					if (levelData.gameObject.activeSelf)
					{
						levelData.gameObject.SetActive(false);
					}
					i++;
				}
			}
		}

		public LevelData GetLevel(int level)
		{
			return levels[level];
		}
    }
}
