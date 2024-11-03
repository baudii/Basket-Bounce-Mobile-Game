using UnityEngine;



namespace BasketBounce.Systems
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