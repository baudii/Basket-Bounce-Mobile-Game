using UnityEngine;
using UnityEngine.UI;

namespace KK
{
    public class FlexibleGridLayout : LayoutGroup
    {
        // Определяет какой параметр будет изменяться, чтобы таблица уместилась в экране (прим.: по горизонтали, если выбран FixedSide.Rows)
        // Пример: Допустим, мы хотим зафиксировать 5 столбцов, размер ячейки 30х30 ширина вьюпорта 150, спейсинг 10. (30*5) + 10*4 = 190 > 150.
        // Если мы хотим сохранить ровно 5 столбоцов, то все в этот вьюпорт не влезет. 
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

        [SerializeField, Tooltip("Applies to the main gameObject. Should it to content?")] FitToContent fitToContent;

        [Header("Fix one side")]
        [SerializeField, Tooltip("123")] FixedSide fixedSide;
        [SerializeField, Min(1)] int columns = 1;
        [SerializeField, Min(1)] int rows = 1;
        [Header("Fit the fixed side")]
        [SerializeField] FittingMethod fittingMethod;
        [SerializeField] Vector2 cellSize;
        [SerializeField] Vector2 spacing;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

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
                    if (nonCellsX < containerWidth)
                        cellSize.x = (containerWidth - nonCellsX) / columns;
                }
                else if (fixedSide == FixedSide.Rows)
                {
                    float containerHeight = rectTransform.rect.height;
                    float nonCellsY = (padding.top + padding.bottom + spacing.y * (rows - 1));
                    if (nonCellsY < containerHeight)
                        cellSize.y = (containerHeight - nonCellsY) / rows;
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
                    if (nonSpacingX < containerWidth)
                        spacing.x = totalSpacing / amountOfSpaces;
                }
                else if (fixedSide == FixedSide.Rows && rows > 1)
                {
                    float nonSpacingY = (padding.top + padding.bottom + cellSize.y * rows);
                    float containerHeight = rectTransform.rect.height;

                    if (nonSpacingY < containerHeight)
                        spacing.y = (containerHeight - nonSpacingY) / (rows - 1);
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
                var width = padding.left + padding.right + spacing.x * (columns - 1) + cellSize.x * (columns);

                sizeDelta.x = width;
            }

            if (fitToContent == FitToContent.Height || fitToContent == FitToContent.Both)
            {
                var height = padding.top + padding.bottom + spacing.y * (rows - 1) + cellSize.y * (rows);

                sizeDelta.y = height;
            }
            
            // Работает только если размер изменяется через стандартные (ширина/высота), а не через "якори"
            rectTransform.sizeDelta = sizeDelta;
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
