// The MIT License (MIT)
// Copyright © 2022 Cosmo7/Eddie Bowen

// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation
// files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy,
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the
// Software.

// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
// COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using UnityEngine;


namespace Cosmo7
{
	public class TweenMaker : MonoBehaviour
	{
		private float startTime;					// tweens are tracked by comparing startTime to Time.unscaledTime
		private float duration;                     // duration is in seconds											
		private EasingDirection easingDirection;

		public Func<float, float> easingFunction;

		//	output actions
		public Action<float> onUpdate;				// called every update with a value between 0.0 and 1.0
		public Action onComplete;					// called when tween is complete

		//	Static constructor
		//		* each tween is attached to an existing GameObject and uses that object's Update phase
		//		* if that object is destroyed then no more updates will be called 
		//		* the tween destroys itself when it is complete

		public static TweenMaker Create(Component component, float duration = 1.0f, Easing easing = Easing.Linear, EasingDirection easingDirection = EasingDirection.easeOut)
		{
			var tween = component.gameObject.AddComponent<TweenMaker>();
			tween.startTime = Time.unscaledTime;
			tween.duration = duration;
			tween.easingFunction = GetEasingFunction(easing);
			tween.easingDirection = easingDirection;

			return tween;
		}

		private static Func<float, float> GetEasingFunction(Easing easing)
		{
			// adapted from https://easings.net/

			switch (easing)
			{
				default:
				case Easing.Linear:
					return (p) => { return p; };

				case Easing.Quadratic:
					return (p) => { return Mathf.Pow(p, 2); };

				case Easing.Cubic:
					return (p) => { return Mathf.Pow(p, 3); };

				case Easing.Quartic:
					return (p) => { return Mathf.Pow(p, 4); };

				case Easing.Quintic:
					return (p) => { return Mathf.Pow(p, 5); };

				case Easing.Sine:
					return (p) => { return 1.0f - Mathf.Cos((p * Mathf.PI) / 2.0f); };

				case Easing.Circular:
					return (p) => { return 1.0f - Mathf.Sqrt(1.0f - (p * p)); };

				case Easing.Exponential:
					return (p) => { return Mathf.Pow(2.0f, 10.0f * (p - 1.0f)); };

				case Easing.Back:
					return (p) => { return Mathf.Pow(p, 3.0f) - (p * Mathf.Sin(p * Mathf.PI)); };

				case Easing.Elastic:
					return (p) => { return Mathf.Sin(6.5f * Mathf.PI * p) * Mathf.Pow(2.0f, 10.0f * (p - 1.0f)); };

				case Easing.Bounce:
					return (p) => { return Mathf.Abs(Mathf.Sin(6.5f * Mathf.PI * p) * Mathf.Pow(2.0f, 10.0f * (p - 1.0f))); };

				case Easing.Custom:
					// function will be supplied by user
					return null;
			}
		}

		public void Update()
		{
			// compare elapsed time to duration
			var elapsed = Time.unscaledTime - startTime;

			if (elapsed >= duration)
			{
				// complete; do a final update to finish the animation neatly
				InvokeUpdate(1.0f);
				InvokeComplete();

				// remove from owner
				Destroy(this);
			}
			else
			{
				// normal update
				InvokeUpdate(Ease(elapsed / duration));
			}
		}


		// Invokers

		private void InvokeUpdate(float ratio)
		{
			if (onUpdate != null)
			{
				onUpdate.Invoke(ratio);
			}
		}

		private void InvokeComplete()
		{
			if (onComplete != null)
			{
				onComplete.Invoke();
			}
		}

		// Easing

		private float Ease(float value)
		{
			switch (easingDirection)
			{
				case EasingDirection.easeIn:
					// apply formula directly
					return EasedValue(value);

				case EasingDirection.easeOut:
					// apply formula backwards
					return 1.0f - EasedValue(1.0f - value);

				default:
				case EasingDirection.easeInOut:
					// apply first half forwards and second half backwards
					if (value < 0.5f)
					{
						return EasedValue(value * 2.0f) / 2.0f;
					}
					else
					{
						return 1.0f - (EasedValue((1.0f - value) * 2.0f) / 2.0f);
					}
			}
		}

		private float EasedValue(float p)
		{
			if (easingFunction != null)
			{
				return easingFunction.Invoke(p);
			}

			// show a warning
			Debug.LogWarning("TweenMaker custom function not set");
			return p;
		}
	}

	public enum Easing
	{
		Linear,
		Quadratic,
		Cubic,
		Quartic,
		Quintic,
		Sine,
		Circular,
		Exponential,
		Elastic,
		Back,
		Bounce,
		Custom,
	}

	public enum EasingDirection
	{
		easeIn,
		easeOut,
		easeInOut
	}
}
