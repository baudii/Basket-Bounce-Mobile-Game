using KK.Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BasketBounce.Gameplay.Levels
{
    public class LevelSet : MonoBehaviour
    {
		[SerializeField] List<LevelChunk> chunks;
		[SerializeField] int chunkSize;
		[SerializeField] int levelSetId;
		public int LevelSetId => levelSetId;

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
					this.Log("Level count:", _levelCount);
				}

				return (int)_levelCount;
			}
		}

		public void InitChunk(int level)
		{
			this.Log("Init with level", level);
			foreach (var chunk in chunks)
				chunk.ValidateChunk();

			currentChunkIndex = level / chunkSize;
			currentChunk = Instantiate(chunks[currentChunkIndex], transform);
		}

		public LevelData GetLevel(int level)
		{
			if (level / chunkSize != currentChunkIndex) // If level is not in current chunk
				SwapChunks(level);

			return currentChunk.GetLevel(level % chunkSize);
		}

		public void SwapChunks(int level)
		{
			this.Log($"Swapping level chunk. Level: {level}, chunkSize: {chunkSize}, index: {level / chunkSize}");
			currentChunkIndex = level / chunkSize;
			Destroy(currentChunk.gameObject);
			currentChunk = Instantiate(chunks[currentChunkIndex], transform);
		}
	}
}
