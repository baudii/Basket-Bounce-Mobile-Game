namespace BasketBounce.Models
{
	public struct ScoreData
	{
		public int bounces;
		public int stars;
		public int nextStarBounceRequirement;

		public ScoreData(int stars, int nextStarBounceRequirement, int bounces)
		{
			this.bounces = bounces;
			this.stars = stars;
			this.nextStarBounceRequirement = nextStarBounceRequirement;
		}
		/* Авось пригодится ?
	   // Знаки сравнения перевернуты, потому что формула расчета score тем больше, чем больше времени и баунсов.
	   // А в игре надо минимизировать показатели. Можно было развернуть дробь, но так проще и результат тот же
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

}