![Logo160](https://user-images.githubusercontent.com/2846899/159929716-ee7d2187-8974-4a86-b2f0-062ef74894c6.png)
# TweenMachine
A minimal, action-based tween utility for Unity

## What does action-based mean?
With TweenMachine all tweening is done through an update action called ```onUpdate```. This action has a single parameter: an eased value that represents how complete the tween is. What you do with that value is up to you; it's perfect for feeding into Unity's Lerp and Slerp functions.

TweenMachine does *not* have functions to move, rotate, scale, fade, set colors, manage objects, fold laundry, etc. You do everything yourself, in the ```onUpdate``` and ```onComplete``` functions.

This is better for you (because you don't have to trawl through documentation to find out how to do something), and it's better for me (because I don't have to write the documentation).

## Features
* Single file
* Open source, MIT license, on GitHub
* Function-oriented interface
* Nice selection of easings
* Option to chain tweens

## Typical use

Add the using:
```
using TweenMachine;
```

Then, somewhere:
```
// make the tween
var tween = transform.Tween(this);
tween.duration = 0.75f;
tween.easing = Easing.Quadratic;
tween.easingDirection = EasingType.easeInOut;

// define what the tween is doing
var startPosition = transform.localPosition;
var endPosition = new Vector3(0, 0, 100);

// create the update callback
tween.onUpdate = (t) =>
{
	// t ranges from 0.0f to 1.0f over the duration of the tween
	transform.localPosition = Vector3.LerpUnclamped(startPosition, endPosition, t);
};
```
The tween component is added and immediately starts running, and destroys itself after calling ```onComplete```.

## Easing Options
TweenMachine offers the following types of easing, as described at https://easings.net/
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
