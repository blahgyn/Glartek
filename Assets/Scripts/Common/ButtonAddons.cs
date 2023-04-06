using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

/// <summary>
/// Class to create an animation in buttons
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonAddons : MonoBehaviour,
	IPointerEnterHandler,
	IPointerExitHandler
{
	[SerializeField] readonly bool ShouldAnimate = true;
	[SerializeField] Vector2 BaseScale  = Vector2.one;
	[SerializeField] Vector2 HoverScale = new(1.05f, 1.05f);

	private Button button;

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (button is null) button = GetComponent<Button>();

		if (!ShouldAnimate) return;
		if (!button.interactable || !button.enabled) return;

		transform.DOScale(HoverScale, .1f);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (button is null) button = GetComponent<Button>();

		if (!ShouldAnimate) return;
		if (!button.interactable || !button.enabled) return;

		transform.DOScale(BaseScale, .1f);
	}
}
