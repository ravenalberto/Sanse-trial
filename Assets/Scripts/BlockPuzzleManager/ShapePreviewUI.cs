using UnityEngine;
using UnityEngine.EventSystems;

public class ShapePreviewUI : MonoBehaviour, IPointerClickHandler
{
	public int index;
	public BlockPuzzleManager manager;

	public void OnPointerClick(PointerEventData eventData)
	{
		manager.SelectShape(index);
	}
}