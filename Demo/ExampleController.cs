using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cosmo7;


public class ExampleController : MonoBehaviour
{
	public Transform gameObject1;
	public Image image2;
	public Transform gameObject3;
	public Transform gameObject4;

	
	public void Example1()
	{
		// simple example that moves a GameObject from a to b

		var tween = TweenMaker.Create(this, 2.0f, Easing.Back, EasingDirection.easeInOut);

		// set up a and b
		var positionA = new Vector3(2.0f, 1.0f, -1.0f);
		var positionB = new Vector3(-2.0f, 1.0f, -1.0f);

		// set up the update function
		tween.onUpdate = (t) =>
		{
			gameObject1.localPosition = Vector3.LerpUnclamped(positionA, positionB, t);
		};
	}

	public void Example2()
	{
		// simple example that tweens an image color

		var tween = TweenMaker.Create(this, 3.0f, Easing.Cubic, EasingDirection.easeInOut);

		tween.onUpdate = (t) =>
		{
			image2.color = Color.LerpUnclamped(Color.red, Color.green, t);
		};
	}

	public void Example3()
	{
		// more complex example that uses onComplete to issue a second tween

		var tween = TweenMaker.Create(this, 3.0f, Easing.Elastic, EasingDirection.easeOut);

		var startRotation = Quaternion.Euler(0, 0, 0);
		var endRotation = Quaternion.Euler(45, 60 ,-20);

		tween.onUpdate = (t) =>
		{
			// rotate from start to end
			gameObject3.rotation = Quaternion.SlerpUnclamped(startRotation, endRotation, t);
		};

		tween.onComplete = () =>
		{
			// do another tween totating the object back to the start
			var secondTween = TweenMaker.Create(this, 0.5f, Easing.Elastic, EasingDirection.easeOut);

			secondTween.onUpdate = (t) =>
			{
				// rotate back from end to start
				gameObject3.rotation = Quaternion.SlerpUnclamped(endRotation, startRotation, t);
			};
		};
	}

	public void Example4()
	{
		// more complex example that uses a custom easing function

		var tween = TweenMaker.Create(this, 4.5f, Easing.Custom, EasingDirection.easeOut);

		tween.easingFunction = (p) => {
			return Mathf.Sin(8.5f * Mathf.PI * p) * Mathf.Pow(3.0f, 15.0f * (p - 1.0f));
		};

		var startPosition = new Vector3(2.0f, 1.0f, 2.1f);
		var endPosition = new Vector3(-1.0f, 1.0f, 2.1f);
		
		tween.onUpdate += (t) =>
		{
			// move and rotate object
			gameObject4.localPosition = Vector3.LerpUnclamped(startPosition, endPosition, t);

			var rotation = Mathf.LerpUnclamped(0.0f, 60.0f, t);
			gameObject4.rotation = Quaternion.Euler(-rotation, rotation, 0.0f);

			// do something weird with scale to show it doesn't have to just be lerps everywhere
			gameObject4.localScale = Vector3.one * (0.5f + (Mathf.Sin(t * Mathf.PI * 2.0f) * 0.25f));
		};
	}

}

