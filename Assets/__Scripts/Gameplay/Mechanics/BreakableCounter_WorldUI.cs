using TMPro;
using UnityEngine;

namespace BasketBounce.UI
{
	public class BreakableCounter_WorldUI : MonoBehaviour
	{
		[SerializeField] TMP_Text text;
		public void SetText(string value)
		{
			text.text = value.ToString();
		}
	}
}

