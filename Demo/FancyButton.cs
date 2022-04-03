using UnityEngine;
using UnityEngine.UI;
using TweenMachine;
using UnityEngine.EventSystems;

public class FancyButton : Button
{


	public override void OnPointerClick(PointerEventData eventData)
	{
		base.OnPointerClick(eventData);

		var tween = gameObject.Tween();
		tween.duration = 0.25f;

		tween.onUpdate = (t) =>
		{
			var scale = (Mathf.Cos(t * Mathf.PI) + 1.0f) * 0.5f;
			transform.localScale = Vector3.one * scale;
		};
	}
}

