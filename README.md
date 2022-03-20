# TweenMaker
A minimal, function-oriented tween utility for Unity

## What makes TweenMaker different?
TweenMaker is different from other tweening libraries because all of the actual logic for tweens is external. TweenMaker uses two callbacks, ```onUpdate``` and ```onComplete```; all of your tweened logic goes in there. There's no long API of move and fade and rotate calls, just the two callback functions. 

It's as simple as it possibly can be.

## Features
* Single file
* Function-oriented interface
* Nice selection of easings

## Limitations
TweenMaker is a MonoBehaviour that adds itself to a GameObject; you can't use TweenMaker without a GameObject.

## Typical use

Add the using:
```
using Cosmo7;
```

Then, somewhere:
```
var startPosition = transform.localPosition;
var endPosition = new Vector3(0, 0, 100);

// make the tween
var tween = TweenMaker.Create(this);
tween.duration = 0.75f;
tween.easing = Easing.Quadratic;
tween.easingType = EasingType.easeInOut;

// create the update callback
tween.onUpdate += (t) =>
{
	// t ranges from 0.0f to 1.0f over the duration of the tween
	// use LerpUnclamped rather than Lerp because some easings (eg: Elastic) can return values outside 0.0..1.0
	transform.localPosition = Vector3.LerpUnclamped(startPosition, endPosition, t);
};

// (optionally) create the completion callback
tween.onComplete += () =>
{
	Debug.Log("Tween complete!);
};
```
The tween component is added and immediately starts running, and destroys itself after calling onComplete.

## Easing Options
TweenMaker offers the following types of easing, as described at https://easings.net/
* Linear
* Quadratic
* Cubic
* Quartic
* Quintic
* Sine
* Circular
* Exponential
* Elastic
* Back
* Bounce

These easings can be applied as
* In
* Out
* In-Out
