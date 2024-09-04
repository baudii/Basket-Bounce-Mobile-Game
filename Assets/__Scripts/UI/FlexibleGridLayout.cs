using System.Drawing;
using UnityEngine;
using UnityEngine.UI;

namespace KK
{
    public class FlexibleGridLayout : LayoutGroup
    {

        enum FittingMethod
        {
            None, Cell, Spacing
        }

        enum FixedDirection
        {
            Horizontal, Vertical
        }

        enum FitToContent
        {
            None, Width, Height, Both
        }

        [SerializeField] FittingMethod fittingMethod;
        [SerializeField] Vector2 cellSize;
        [SerializeField] bool FitToContentSize;

        [SerializeField] FixedDirection fixedDirection;
        [SerializeField, Min(1)] int columns = 1;
        [SerializeField, Min(1)] int rows = 1;
        [SerializeField] Vector2 spacing;
        [SerializeField] FitToContent fitToContent;

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            if (fixedDirection == FixedDirection.Horizontal)
            {
                rows = transform.childCount / columns + (int)Mathf.Sign(transform.childCount % columns);
            }
            else if (fixedDirection == FixedDirection.Vertical)
            {
                columns = transform.childCount / rows + (int)Mathf.Sign(transform.childCount % rows);
            }
            if (fittingMethod == FittingMethod.Cell)
            {
                if (fixedDirection == FixedDirection.Horizontal)
                {
                    float containerWidth = rectTransform.rect.width;
                    float nonCellsX = (padding.left + padding.right + spacing.x * (columns - 1));
                    if (nonCellsX < containerWidth)
                        cellSize.x = (containerWidth - nonCellsX) / columns;
                }
                else if (fixedDirection == FixedDirection.Vertical)
                {
                    float containerHeight = rectTransform.rect.height;
                    float nonCellsY = (padding.top + padding.bottom + spacing.y * (rows - 1));
                    if (nonCellsY < containerHeight)
                        cellSize.y = (containerHeight - nonCellsY) / rows;
                }
            }
            else if (fittingMethod == FittingMethod.Spacing)
            {
                if (fixedDirection == FixedDirection.Horizontal && columns > 1)
                {
                    float containerWidth = rectTransform.rect.width;
                    float nonSpacingX = (padding.left + padding.right + cellSize.x * columns);

                    if (nonSpacingX < containerWidth)
                        spacing.x = (containerWidth - nonSpacingX) / (columns - 1);
                }
                else if (fixedDirection == FixedDirection.Vertical && rows > 1)
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
            Vector2 pos = rectTransform.anchoredPosition;

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

            rectTransform.sizeDelta = sizeDelta;
            rectTransform.anchoredPosition = pos;
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
