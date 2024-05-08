using UnityEngine;
using UnityEngine.UI;

public class GridManager : LayoutGroup
{
	public int rows;
	public int columns;

	[SerializeField] private CardGrid cardGrid;

	public Vector2 cardSize;
	public Vector2 spacing;
	public int topPadding;


	public override void CalculateLayoutInputVertical()
	{
		rows = cardGrid.rowsValue;
		columns = cardGrid.columnsValue;

		float parentWidth = rectTransform.rect.width;
		float parentHeight = rectTransform.rect.height;

		float cardHeight = (parentHeight - 2 * topPadding - spacing.y * (columns - 1)) / columns;
		float cardWidth = parentHeight / rows;

		if(cardWidth * rows + spacing.x * (rows - 1) > parentWidth)
		{
			cardWidth = parentWidth - 2 * topPadding - (rows - 1) * spacing.x / rows;
			cardHeight = cardWidth;
		}

		cardSize = new Vector2(cardWidth, cardHeight);

		padding.left = Mathf.FloorToInt((parentWidth - rows * cardWidth - spacing.x * (rows - 1)) / 2);
		padding.top = Mathf.FloorToInt((parentHeight - columns * cardHeight - spacing.y * (columns - 1)) / 2);
		padding.bottom = padding.top;

		for (int i = 0; i < rectChildren.Count; i++)
		{
			int rowCount = i / rows;
			int rowsCount = i % rows;

			var item = rectChildren[i];

			var xPos = padding.left + cardSize.x * rowsCount + spacing.x * (rowsCount - 1);
			var yPos = padding.top + cardSize.y * rowCount + spacing.y * (rowCount);

			SetChildAlongAxis(item, 0, xPos, cardSize.x);
			SetChildAlongAxis(item, 1, yPos, cardSize.y);
		}
	}

	public override void SetLayoutHorizontal()
	{
		return;
	}

	public override void SetLayoutVertical()
	{
		return;
	}
}
