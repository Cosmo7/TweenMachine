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


using UnityEngine;


namespace Cosmo7
{
	public class TweenMaker : MonoBehaviour
	{
		private float startTime;                    // tweens are tracked by comparing startTime to Time.unscaledTime

		// public values
		public float duration = 1.0f;               // duration is in seconds
		public Easing easing = Easing.Linear;
		public EasingType easingType = EasingType.easeIn;

		// output actions
		public System.Action<float> onUpdate;       // called every update with a value between 0.0 and 1.0
		public System.Action onComplete;            // called when tween is complete

		//	each tween is attached to an existing GameObject and uses that object's Update phase
		//	if that object is destroyed then no more updates will be called 
		//	the tween destroys itself when it is complete

		// Constructors

		public static TweenMaker Create(GameObject owner)
		{
			var tween = owner.AddComponent<TweenMaker>();
			tween.startTime = Time.unscaledTime;
			return tween;
		}

		public static TweenMaker Create(Component component)
		{
			// convenience function
			return Create(component.gameObject);
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
			// only ease intermediate values
			if (value == 0.0f || value == 1.0f) return value;

			switch (easingType)
			{
				case EasingType.easeIn:
					// apply formula directly
					return EasedValue(value);

				case EasingType.easeOut:
					// apply formula backwards
					return 1.0f - EasedValue(1.0f - value);

				default:
				case EasingType.easeInOut:
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
			// adapted from https://easings.net/

			switch (easing)
			{
				default:
				case Easing.Linear:
					return p;

				case Easing.Quadratic:
					return Mathf.Pow(p, 2);

				case Easing.Cubic:
					return Mathf.Pow(p, 3);

				case Easing.Quartic:
					return Mathf.Pow(p, 4);

				case Easing.Quintic:
					return Mathf.Pow(p, 5);

				case Easing.Sine:
					return 1.0f - Mathf.Cos((p * Mathf.PI) / 2.0f);

				case Easing.Circular:
					return 1.0f - Mathf.Sqrt(1.0f - (p * p));

				case Easing.Exponential:
					return Mathf.Pow(2.0f, 10.0f * (p - 1.0f));

				case Easing.Back:
					return Mathf.Pow(p, 3.0f) - (p * Mathf.Sin(p * Mathf.PI));

				case Easing.Elastic:
					return Mathf.Sin(6.5f * Mathf.PI * p) * Mathf.Pow(2.0f, 10.0f * (p - 1.0f));

				case Easing.Bounce:
					return Mathf.Abs(Mathf.Sin(6.5f * Mathf.PI * p) * Mathf.Pow(2.0f, 10.0f * (p - 1.0f)));
			}
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
	}

	public enum EasingType
	{
		easeIn,
		easeOut,
		easeInOut
	}
}
