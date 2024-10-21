using UnityEngine;
using UnityEngine.Events;

namespace KK.Common
{
	public class OnTriggerEnter2DCustomEvent : MonoBehaviour
	{
		[SerializeField] UnityEvent _event;

		void OnTriggerEnter2D(Collider2D collision)
		{
			_event?.Invoke();
		}
	}
}