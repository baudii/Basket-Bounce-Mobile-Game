using UnityEngine;



namespace BasketBounce.SO
{
	[CreateAssetMenu(fileName = "Game Assets", menuName = "SO/Game Assets")]
	public class GameAssets_SO : ScriptableObject
	{
		[SerializeField] AudioClip blopSound;
		[SerializeField] AudioClip winSound;
		public AudioClip BlopSound => blopSound;
		public AudioClip WinSound => winSound;
	}
}