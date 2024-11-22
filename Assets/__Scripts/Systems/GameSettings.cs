using UnityEngine;

namespace BasketBounce.Systems
{
	[CreateAssetMenu(fileName = "GameSettings", menuName = "Settings/GameSettings")]
    public class GameSettings : ScriptableObject
    {
		[SerializeField] bool autostartEnabled;
		public bool AutoStartEnabled => autostartEnabled;
    }
}
