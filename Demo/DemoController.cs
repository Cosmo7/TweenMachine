using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cosmo7;


public class DemoController : MonoBehaviour
{
	public float duration;
	public Easing easing;
	public EasingType easingType;
	public Transform movingObject;

	public Text feedback;
	public Text easingLabel;
	public Text easingTypeLabel;
	public Text durationLabel;
	public Slider durationSlider;

	public void Start()
	{
		SetDuration(2.0f);
		UpdateLabels();
	}

	// this is the actual tween code
	public void Animate()
	{
		// make the tween
		var tween = TweenMaker.Create(this);
		tween.duration = duration;
		tween.easing = easing;
		tween.easingType = easingType;

		// the tween rotates and translates the object
		var startPosition = new Vector3(-2.0f, 1.0f, 0.0f);
		var endPosition = new Vector3(2.0f, 1.0f, 0.0f);
		var startRotation = 0.0f;
		var endRotation = 60.0f;

		tween.onUpdate += (t) =>
		{
			// use unclamped lerps because some easings go outside 0.0f..1.0f
			movingObject.position = Vector3.LerpUnclamped(startPosition, endPosition, t);

			var rotation = Mathf.LerpUnclamped(startRotation, endRotation, t);
			movingObject.rotation = Quaternion.Euler(-rotation, rotation, 0.0f);

			SetFeedbackText(t);
		};

		// onComplete is optional; onUpdate always does a final call at (t = 1.0f) so that the tween
		// completes neatly. You can use onComplete to tidy up, to chain subsequent tweens, etc.

		tween.onComplete += () =>
		{
			// go back to the start
			movingObject.position = startPosition;
			movingObject.rotation = Quaternion.identity;
			SetFeedbackText(0.0f);
		};
	}

	// UI and input stuff

	private void SetFeedbackText(float t)
	{
		feedback.text = string.Format("t = {0:0.##}f", t);
	}

	public void ChangeDuration()
	{
		SetDuration(durationSlider.value);
	}

	private void SetDuration(float newDuration)
	{
		duration = newDuration;
		durationLabel.text = string.Format("Duration: {0:0.##}", duration);
	}

	public void NextEasing()
	{
		easing = (Easing)Next<Easing>(easing);
		UpdateLabels();
	}

	public void NextEasingType()
	{
		easingType = (EasingType)Next<EasingType>(easingType);
		UpdateLabels();
	}

	private void UpdateLabels()
	{
		easingLabel.text = string.Format("Easing: {0}", easing);
		easingTypeLabel.text = string.Format("EasingType: {0}", easingType);
	}

	private int Next<T>(T value)
	{
		// this is just a quick way of rotating enum values to make the UI simpler
		var values = System.Enum.GetValues(typeof(T));
		var index = System.Array.IndexOf(values, value);
		index++;

		if(index >= values.Length)
		{
			index = 0;
		}

		return index;
	}
}

