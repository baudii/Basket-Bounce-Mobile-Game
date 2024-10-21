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
			// ��������� ����������
			Vector2 bouncePosition;
			float currentLenght = 0;
			float currentMaxLength = maxLength;
			int reflections = 0;

			// ���������� ��������� ����������� �����, ���� ��� ������� �����
			currentMaxLength *= Mathf.Clamp(stretchForce, 0, 1);

			// ������� ���������� ������ �� ��������� ����������, ����� �������� �� ���������� LineRenderer
			List<Vector3> positions = new List<Vector3>();
			positions.Add(start);
			start += dir * ballRadius;

			// ����� �������������� �������� ������� �����
			if (maxReflections <= 0)
			{
				maxReflections = 4;
			}

			while (currentLenght < currentMaxLength)
			{
				// Circle Cast, ��������� �� ������� ���. ballRadius = Ball.Collider.radius * Ball.transform.localScale 
				RaycastHit2D hit = Physics2D.CircleCast(start, ballRadius, dir, currentMaxLength - currentLenght, bouncable);

				// ���� ��� �� �������� ������������ - ���������� �����, ���������� �� �������� "���������" ����� � ��������� ������� �� ���������� �����
				if (hit.collider == null)
				{
					Vector3 lastPos = start + dir * (currentMaxLength - currentLenght);

					positions.Add(lastPos);
					break;
				}

				// ��� �������� �����������. ��������� ����������� ���������
				dir = Vector2.Reflect(dir, hit.normal);

				if (hit.collider.isTrigger)
				{
					if (hit.collider.TryGetComponent(out BouncePad bp))
					{
						// ������ ������� ��� ����������� BouncePad
						dir = bp.transform.up.normalized;
					}
					else if (hit.collider.TryGetComponent(out Finish _) || hit.collider.TryGetComponent(out Killable _))
					{
						// ����� � ������ ����������� ��������
						Vector3 lastPos = hit.point;
						positions.Add(lastPos);
						break;
					}
					// TODO: ������, ������� ��������� �����
				}
				// ���� �� �����, ������ ����������� - �����

				// ������� �����, ��������� bouncePosition - ��������� ����������
				bouncePosition = hit.point + hit.normal * ballRadius; // ��������� ������� ������ ���� � ������ ������������
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