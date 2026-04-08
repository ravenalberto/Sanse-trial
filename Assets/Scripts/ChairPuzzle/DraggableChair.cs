using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine.EventSystems;

public class DraggableChair : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public string chairName;
	public bool isInSlot = false;

	private RectTransform rectTransform;
	private Canvas canvas;

	private Vector2 originalPosition;

	private void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
		canvas = GetComponentInParent<Canvas>();
		originalPosition = rectTransform.anchoredPosition;
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		transform.SetAsLastSibling();
	}

	public void OnDrag(PointerEventData eventData)
	{
		rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
	}

	public GridSlot currentSlot;

	public GridSlot[] slots;

	public void OnEndDrag(PointerEventData eventData)
	{
		float closestDistance = float.MaxValue;
		GridSlot closestSlot = null;

		foreach (GridSlot slot in slots)
		{
			float distance = Vector2.Distance(
				rectTransform.position,
				slot.transform.position
			);

			if (distance < closestDistance)
			{
				closestDistance = distance;
				closestSlot = slot;
			}
		}

		// SNAP IF VALID
		if (closestSlot != null && closestDistance < 150f && closestSlot.currentChair == null)
		{
			// CLEAR OLD SLOT
			if (currentSlot != null)
			{
				currentSlot.currentChair = null;
			}

			// SNAP POSITION
			rectTransform.position = closestSlot.transform.position;

			// ASSIGN NEW SLOT
			closestSlot.currentChair = this;
			currentSlot = closestSlot;

			isInSlot = true;

			Debug.Log(chairName + " placed in " + closestSlot.name);
		}
		else
		{
			// CLEAR OLD SLOT
			if (currentSlot != null)
			{
				currentSlot.currentChair = null;
				currentSlot = null;
			}

			// RETURN
			rectTransform.anchoredPosition = originalPosition;
			isInSlot = false;

			Debug.Log(chairName + " returned");
		}
	}
}