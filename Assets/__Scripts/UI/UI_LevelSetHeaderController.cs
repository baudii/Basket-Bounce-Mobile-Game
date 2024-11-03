using KK.Common;
using TMPro;
using UnityEngine;

namespace BasketBounce.UI
{
    public class UI_LevelSetHeaderController : MonoBehaviour
    {
		[SerializeField] TextMeshProUGUI tmp;

		string[] headers = new string[]
		{
			"Library", "Maze", "Kitchen"
		};

		public void SetHeader(int levelSet)
		{
			tmp.text = headers[levelSet];
		}
    }
}
