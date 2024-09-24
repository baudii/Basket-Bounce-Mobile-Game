using UnityEngine;

public class UI_TweenPosition : MonoBehaviour
{
	RectTransform rectToMove;

	private void OnEnable()
	{
		Move();
	}

	public void Move()
	{
		// установить в начальную позицию
		// санимировать
		// отключиться
		rectToMove = (RectTransform)transform;
		var initial = transform.position;
		rectToMove.position = new Vector3(100, 0, 0);
		//transform.DOMove(initial, 1f);
	}
}
