using UnityEngine;
using UnityEngine.UI;
using Cosmo7;
using UnityEngine.EventSystems;

public class FancyButton : Button
{


	public override void OnPointerClick(PointerEventData eventData)
	{
		base.OnPointerClick(eventData);

		var tween = TweenMaker.Create(gameObject);
		tween.duration = 0.25f;
		tween.easing = Easing.Sine;
		tween.easingDirection = EasingDirection.easeIn;
		tween.pingPong = true;

		tween.onUpdate = (t) =>
		{
			transform.localScale = (1.0f + (t * 0.1f)) * Vector3.one;
		};
	}
}

