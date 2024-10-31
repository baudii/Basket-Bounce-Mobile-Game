using KK.Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BasketBounce.Systems
{
    public class LevelSet : MonoBehaviour
    {
		[SerializeField] List<LevelChunk> chunks;
		[SerializeField] int chunkSize;

		LevelChunk currentChunk;
		int currentChunkIndex;

		private int? _levelCount;
		public int LevelCount
		{
			get
			{
				if (_levelCount == null)
				{
					var lastLevelCount = chunks.Last().LevelCount;
					_levelCount = chunkSize * (chunks.Count - 1) + lastLevelCount;
					this.SmartLog("Level count:", _levelCount);
				}

				return (int)_levelCount;
			}
		}

		public void Init(int level)
		{
			currentChunkIndex = level / chunkSize;
			currentChunk = Instantiate(chunks[0], transform);
			
			chunkSize = currentChunk.LevelCount;
		}

		public LevelData GetLevel(int level)
		{
			if (level / chunkSize != currentChunkIndex)
			{
				currentChunkIndex = level / chunkSize;
				Destroy(currentChunk.gameObject);
				currentChunk = Instantiate(chunks[currentChunkIndex], transform);
			}
			return currentChunk.GetLevel(level % chunkSize);
		}
	}
}
