using UnityEngine;
using UnityEngine.UI;

namespace KK.Common
{
	public class FlexibleGridLayout : LayoutGroup
	{
		enum FittingMethod
		{
			None, Cell, Spacing
		}

		// Какую сторону таблицы мы хотим фиксировать. Другая будет вычисленна 
		enum FixedSide
		{
			Columns, Rows
		}

		enum FitToContent
		{
			None, Width, Height, Both
		}

		[SerializeField, Tooltip("Applies to the this transform. Acts similar to ContentSizeFitter")] FitToContent fitToContent;

		[Header("Fix one side")]
		[SerializeField] FixedSide fixedSide;
		[SerializeField, Min(1)] int columns = 1;
		[SerializeField, Min(1)] int rows = 1;
		[Header("Fit the fixed side")]
		[SerializeField] FittingMethod fittingMethod;
		[SerializeField] Vector2 cellSize;
		[SerializeField] Vector2 spacing;
		[Header("Set boundaries")]
		[SerializeField, Tooltip("x=left, y=right, z=top, w=bottom")] Point4Int minPading;
		[SerializeField, Min(0)] float minSpacing, maxSpacing, minCellSize, maxCellSize;

		public override void CalculateLayoutInputHorizontal()
		{
			base.CalculateLayoutInputHorizontal();

			padding.left = minPading.x;
			padding.right = minPading.y;
			padding.top = minPading.z;
			padding.bottom = minPading.w;

			// Полагая, что разработчик уже зафиксировал один из параметров (столбцы или строки), вычисляем неизвестный, основываясь на количестве элементов
			if (fixedSide == FixedSide.Columns)
			{
				rows = transform.childCount / columns + Utils.Signum(transform.childCount % columns);
			}
			else if (fixedSide == FixedSide.Rows)
			{
				columns = transform.childCount / rows + Utils.Signum(transform.childCount % rows);
			}

			// Пробуем вместить элементы по ширине или высоте по одному из указанных методов (либо меняя размер самой ячейки, либо меняя расстояние между ячейками)
			if (fittingMethod == FittingMethod.Cell)
			{
				if (fixedSide == FixedSide.Columns)
				{
					float containerWidth = rectTransform.rect.width;
					float nonCellsX = (padding.left + padding.right + spacing.x * (columns - 1)); // Требует пояснения, хоть переменная говорящая. Весь горизонтальный отрезок за вычетом ячеек
					float totalCellLength = (containerWidth - nonCellsX);
					float amountOfCells = columns;

					if (nonCellsX < containerWidth)
						cellSize.x = totalCellLength / amountOfCells;
				}
				else if (fixedSide == FixedSide.Rows)
				{
					float containerHeight = rectTransform.rect.height;
					float nonCellsY = (padding.top + padding.bottom + spacing.y * (rows - 1));
					float totalCellLength = (containerHeight - nonCellsY);
					float amountOfCells = rows;

					if (nonCellsY < containerHeight)
						cellSize.y = totalCellLength / amountOfCells;
				}
			}
			else if (fittingMethod == FittingMethod.Spacing)
			{
				if (fixedSide == FixedSide.Columns && columns > 1)
				{
					float containerWidth = rectTransform.rect.width;
					float nonSpacingX = (padding.left + padding.right + cellSize.x * columns); // Аналогично весь горизнотальный отрезок за вычетом "спейсинга"
					float totalSpacing = (containerWidth - nonSpacingX);
					float amountOfSpaces = (columns - 1);
					float result = totalSpacing / amountOfSpaces;

					float leftOver = (totalSpacing - maxSpacing * amountOfSpaces) * 0.5f;

					result = Mathf.Clamp(result, minSpacing, maxSpacing);
					AdjustPadding(leftOver, true);

					spacing.x = result;
				}
				else if (fixedSide == FixedSide.Rows && rows > 1)
				{
					float nonSpacingY = (padding.top + padding.bottom + cellSize.y * rows);
					float totalSpacing = (rectTransform.rect.height - nonSpacingY);
					float amountOfSpaces = (rows - 1);
					float result = totalSpacing / amountOfSpaces;

					float leftOver = (totalSpacing - maxSpacing * amountOfSpaces) * 0.5f;

					result = Mathf.Clamp(result, minSpacing, maxSpacing);

					spacing.x = result;
					AdjustPadding(leftOver, false);
				}
			}


			for (int i = 0; i < rectChildren.Count; i++)
			{
				var rowCount = i / columns;
				var columnCount = i % columns;

				var item = rectChildren[i];

				var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
				var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

				SetChildAlongAxis(item, 0, xPos, cellSize.x);
				SetChildAlongAxis(item, 1, yPos, cellSize.y);

			}

			Vector2 sizeDelta = rectTransform.sizeDelta;

			if (fitToContent == FitToContent.Width || fitToContent == FitToContent.Both)
			{
				if (rectTransform.anchorMin.x == rectTransform.anchorMax.x)
				{
					var width = padding.left + padding.right + spacing.x * (columns - 1) + cellSize.x * (columns);
					sizeDelta.x = width;
				}
				else
				{
					this.LogWarning("Can't fit to content's width. Wrong anchor's positions!");
					if (fitToContent == FitToContent.Width)
						fitToContent = FitToContent.None;
					else
						fitToContent = FitToContent.Height;
				}
			}

			if (fitToContent == FitToContent.Height || fitToContent == FitToContent.Both)
			{
				if (rectTransform.anchorMin.y == rectTransform.anchorMax.y)
				{
					var height = padding.top + padding.bottom + spacing.y * (rows - 1) + cellSize.y * (rows);
					sizeDelta.y = height;
				}
				else
				{
					this.LogWarning("Can't fit to content's height. Wrong anchor's positions!");
					if (fitToContent == FitToContent.Height)
						fitToContent = FitToContent.None;
					else
						fitToContent = FitToContent.Width;
				}
			}

			// Работает только если размер изменяется через стандартные (ширина/высота), а не через "якори"
			rectTransform.sizeDelta = sizeDelta;
		}

		void AdjustPadding(float leftOver, bool isHorizontal)
		{
			int leftOverInt = Mathf.FloorToInt(leftOver);
			if (isHorizontal)
			{
				padding.left = Mathf.Clamp(padding.left + leftOverInt, minPading.x, padding.left + leftOverInt);
				padding.right = Mathf.Clamp(padding.right + leftOverInt, minPading.y, padding.right + leftOverInt);
			}
			else
			{
				padding.top = Mathf.Clamp(padding.top + leftOverInt, minPading.z, padding.top + leftOverInt);
				padding.bottom = Mathf.Clamp(padding.bottom + leftOverInt, minPading.w, padding.bottom + leftOverInt);
			}
		}

		public override void CalculateLayoutInputVertical()
		{

		}

		public override void SetLayoutHorizontal()
		{

		}

		public override void SetLayoutVertical()
		{

		}

	}
}
