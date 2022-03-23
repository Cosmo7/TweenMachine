using UnityEngine;
using UnityEngine.UI;
using Cosmo7;


public class ExampleController : MonoBehaviour
{
	public Transform mainCamera;

	public GameObject target1;
	public Image target2;
	public GameObject target3;
	public GameObject target4;


	public void Start()
	{
		// do a little camera tween to show user we're running
		var tween = TweenMaker.Create(gameObject);
		tween.easing = Easing.Cubic; 
		tween.easingDirection = EasingDirection.easeOut;
		
		// delay the animation so the scene has a chance to load
		tween.delay = 1.0f;

		var startRotation = mainCamera.rotation;
		var endRotation = Quaternion.Euler(60.0f, 0.0f, 0.0f);

		tween.onUpdate = (t) =>
		{
			mainCamera.rotation = Quaternion.SlerpUnclamped(startRotation, endRotation, t);
		};
	}

	public void Example1()
	{
		// An example that moves a GameObject from one position to another

		var tween = TweenMaker.Create(gameObject);
		tween.duration = 2.0f;
		tween.easing = Easing.Back;
		tween.easingDirection = EasingDirection.easeInOut;

		// set up start and end positions
		var startX = -2.0f;
		var endX = 1.0f;

		tween.onUpdate = (t) =>
		{
			var x = Mathf.LerpUnclamped(startX, endX, t);
			target1.transform.localPosition = new Vector3(x, 0.6f, 0.0f);
		};

		// setting an onComplete function is optional
		tween.onComplete = () =>
		{
			Debug.Log("Help I'm being held prisoner in a tween factory!");
		};
	}

	public void Example2()
	{
		// An example that tweens an image color, pingPongs, and loops

		var tween = TweenMaker.Create(target2.gameObject);
		tween.duration = 3.0f;
		tween.easing = Easing.Cubic;
		tween.easingDirection = EasingDirection.easeInOut;
		tween.pingPong = true;
		tween.loop = true;

		tween.onUpdate = (t) =>
		{
			target2.color = Color.LerpUnclamped(Color.white, Color.blue, t);
		};
	}

	public void Example3()
	{
		// A complex example that chains a second tween

		var tween = TweenMaker.Create(target3);
		tween.duration = 3.0f;
		tween.easing = Easing.Elastic;
		tween.easingDirection = EasingDirection.easeOut;

		var startRotation = Quaternion.Euler(0, 0, 0);
		var endRotation = Quaternion.Euler(45, 60, -20);

		tween.onUpdate = (t) =>
		{
			// rotate from start to end
			target3.transform.rotation = Quaternion.SlerpUnclamped(startRotation, endRotation, t);
		};

		var secondTween = tween.CreateChain();
		secondTween.duration = 0.5f;
		secondTween.easing = Easing.Elastic;
		secondTween.easingDirection = EasingDirection.easeOut;

		secondTween.onUpdate = (t) =>
		{
			// rotate back from end to start
			target3.transform.rotation = Quaternion.SlerpUnclamped(endRotation, startRotation, t);
		};
	}

	public void Example4()
	{
		// An example that uses a custom easing function

		var tween = TweenMaker.Create(target4);
		tween.duration = 4.0f;
		tween.easing = Easing.Custom;
		tween.easingDirection = EasingDirection.easeOut;
		
		tween.SetCustomEasingFunction((p) => {
			return Mathf.Abs(Mathf.Sin(p * Mathf.PI * 6.5f)) * p;
		});
		
		var startPosition = new Vector3(-2.0f, 0.6f, 0.0f);
		var endPosition = new Vector3(1.0f, 0.6f, 0.0f);
		
		tween.onUpdate += (t) =>
		{
			// move and rotate object
			target4.transform.localPosition = Vector3.LerpUnclamped(startPosition, endPosition, t);

			var rotation = Mathf.LerpUnclamped(0.0f, 60.0f, t);
			target4.transform.rotation = Quaternion.Euler(-rotation, rotation, 0.0f);
		};
	}

}

