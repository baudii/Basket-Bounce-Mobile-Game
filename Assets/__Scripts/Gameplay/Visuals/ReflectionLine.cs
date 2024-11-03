using System.Collections.Generic;
using UnityEngine;
using BasketBounce.Gameplay.Mechanics;

namespace BasketBounce.Gameplay.Visuals
{
	public class ReflectionLine : MonoBehaviour
	{
		[SerializeField] float ballRadius;
		[SerializeField] LineRenderer lineRenderer;
		[SerializeField] float maxLength;
		[SerializeField] int maxReflections;
		[SerializeField] LayerMask bouncable;


		public void UpdateReflections(Vector2 start, Vector2 dir, float stretchForce)
		{
			// Объявляем переменные
			Vector2 bouncePosition;
			float currentLenght = 0;
			float currentMaxLength = maxLength;
			int reflections = 0;

			// Равномерно уменьшаем максмальную длину, если мяч натянут слабо
			currentMaxLength *= Mathf.Clamp(stretchForce, 0, 1);

			// Сначала записываем данные во временную переменную, потом передаем их компоненту LineRenderer
			List<Vector3> positions = new List<Vector3>();
			positions.Add(start);
			start += dir * ballRadius;

			// Чтобы гарантированно избежать вечного цикла
			if (maxReflections <= 0)
			{
				maxReflections = 4;
			}

			while (currentLenght < currentMaxLength)
			{
				// Circle Cast, поскольку мы швыряем мяч. ballRadius = Ball.Collider.radius * Ball.transform.localScale 
				RaycastHit2D hit = Physics2D.CircleCast(start, ballRadius, dir, currentMaxLength - currentLenght, bouncable);

				// Если луч не встретил препрятствий - выставляем точку, отдаленную от нынешней "отправной" точки в выбранную сторону на оставшуюся длину
				if (hit.collider == null)
				{
					Vector3 lastPos = start + dir * (currentMaxLength - currentLenght);

					positions.Add(lastPos);
					break;
				}

				// Луч встретил препятствие. Вычисляем направление отражения
				dir = Vector2.Reflect(dir, hit.normal);

				if (hit.collider.isTrigger)
				{
					if (hit.collider.TryGetComponent(out BouncePad bp))
					{
						// Особое условие для поверхности BouncePad
						dir = bp.transform.up.normalized;
					}
					else if (hit.collider.TryGetComponent(out Finish _) || hit.collider.TryGetComponent(out Killable _))
					{
						// Финиш и смерть заканчивают алгоритм
						Vector3 lastPos = hit.point;
						positions.Add(lastPos);
						break;
					}
					// TODO: Кнопка, которая отключает двери
				}
				// Если мы здесь, значит препятствие - стена

				// Порядок важен, поскольку bouncePosition - временная переменная
				bouncePosition = hit.point + hit.normal * ballRadius; // Вычисляем позицию центра мяча в момент столкновения
				currentLenght += Vector2.Distance(start, bouncePosition);
				start = bouncePosition;

				positions.Add(start);

				reflections++;

				if (reflections > maxReflections)
					break;
			}

			lineRenderer.positionCount = positions.Count;
			lineRenderer.SetPositions(positions.ToArray());
		}
	}
}