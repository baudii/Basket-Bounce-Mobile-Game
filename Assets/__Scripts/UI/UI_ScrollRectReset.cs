using UnityEngine;
using UnityEngine.UI;

namespace BasketBounce.UI
{
    public class UI_ScrollRectReset : MonoBehaviour
    {
		[SerializeField] ScrollRect scrollRect;
#if UNITY_EDITOR
		[SerializeField] bool validate;
		[SerializeField, Range(0,1)] float resetValue;
		private void OnValidate()
		{
			if (validate)
				ResetPos();
		}
#endif
		void Start()
        {
			ResetPos();
        }

		void ResetPos()
		{
			scrollRect.horizontalNormalizedPosition = resetValue;
		}
    }
}
