# TweenMaker
A minimal, function-oriented tween[^1] utility for Unity

[^1]:Tweening is an animation process that interpolates values between two frame states. This interpolation can be linear or use a non-linear formula known as easing.

## What makes TweenMaker different?
TweenMaker is different from other tweening libraries because all of the actual logic for tweens is external. TweenMaker uses two callbacks, ```onUpdate``` and ```onComplete```; all of your tweened logic goes in there. There's no long API of move and fade and rotate calls, just the two callback functions. 

It's as simple as it possibly can be.

## Features
* Single file
* Function-oriented interface
* Nice selection of easings

## Limitations
TweenMaker is a MonoBehavior that adds itself to a GameObject; you can't use TweenMaker without a GameObject.

# Typical use

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
tween.type = EasingType.easeInOut;

// create the update callback
tween.onUpdate += (t) =>
{
	// t ranges from 0.0f to 1.0f over the duration of the 
	transform.localPosition = Vector3.LerpUnclamped(startPosition, endPosition, t);
};

// (optionally) create the completion callback
tween.onComplete += () =>
{
	Debug.Log("Tween complete!);
};
```
The tween starts running as soon as it is created.
