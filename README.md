# TweenMaker
Function-oriented tween utility for Unity

Tweening is an animation process that interpolates values between two frame states. This interpolation can be linear or use a non-linear formula known as easing.

TweenMaker is different from other tweening libraries because all of the actual logic for tweens is external. TweenMaker uses two callbacks, ```onUpdate``` and ```onComplete```; all of your tweened logic goes in there. There's no long API of move and fade and rotate calls, just the two callback functions. It's as simple as it possibly can be.

TweenMaker is a MonoBehavior that adds itself to a GameObject; you can't use TweenMaker without a GameObject.

TweenMaker has the following features:
* Single file
* Function-oriented interface
* Multiple easings
* Easing direction is separate from easing type

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

// set the tween's behavior
tween.duration = 0.75f;
tween.easing = Easing.Quadratic;
tween.type = EasingType.easeInOut;

// create the update function
tween.onUpdate += (t) =>
{
	transform.localPosition = Vector3.LerpUnclamped(startPosition, endPosition, t);
};
```
