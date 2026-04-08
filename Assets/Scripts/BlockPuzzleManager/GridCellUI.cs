using UnityEngine;
using UnityEngine.EventSystems;

public class GridCellUI : MonoBehaviour,
	IPointerClickHandler,
	IPointerEnterHandler,
	IPointerExitHandler
{
	public int x;
	public int y;

	public BlockPuzzleManager manager;

	public void OnPointerClick(PointerEventData eventData)
	{
		manager.OnCellClicked(x, y);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		manager.PreviewShape(x, y);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		manager.ClearPreview();
	}
}