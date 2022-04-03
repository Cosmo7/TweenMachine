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
using System.Collections.Generic;
using UnityEngine;



namespace TweenMachine
{
	
	public static class UnityExtensions
	{
		
		public static Tween Tween(this GameObject gameObject)
		{
			return gameObject.AddComponent<Tween>();
		}

		public static Tween Tween(this Component component)
		{
			return component.gameObject.AddComponent<Tween>();
		}
	}


	/// <summary>
	/// A minimal, action-based tween utility for Unity.
	/// </summary>
	/// <remarks>
	/// ## What does action-based mean?
	/// With TweenMaker all tweening is done through an update action called ```onUpdate```. This action has a single parameter: an eased value that represents how complete the tween is. What you do with that value is up to you; it's perfect for feeding into Unity's Lerp and Slerp functions.
	///	
	///	TweenMaker does *not* have functions to move, rotate, scale, fade, set colors, manage objects, fold laundry, etc. You do everything yourself, in the ```onUpdate``` and ```onComplete``` functions.
	/// 
	///	This is better for you(because you don't have to trawl through documentation to find out how to do something), and it's better for me (because I don't have to write the documentation).
	/// </remarks>
	public class Tween : MonoBehaviour
	{
		#region public API

		/// Duration in seconds.
		public float duration = 1.0f;

		/// Delay in seconds.
		public float delay = 0.0f;

		/// <summary>
		/// Specify the easing function.
		/// </summary>
		public Easing easing = Easing.Linear;

		/// Easing direction.
		public EasingDirection easingDirection = EasingDirection.easeOut;

		/// <summary>
		/// Whether a tween with a delay has started.
		/// </summary>
		public bool started = false;

		/// <summary>Optional, called when the Tween starts, after any delay.</summary>
		/// <remarks>
		/// This is useful if you want to set stuff up before the tween starts, but not before any delay or chaining.
		/// </remarks>
		public Action onStart;

		/// <summary>Called every update with an eased value between 0 and 1.</summary>
		/// <remarks>
		/// This is where most of your tween logic will live.
		/// 
		/// Typical usage:
		/// <code>
		/// var startRotation = mainCamera.rotation;
		/// var endRotation = Quaternion.Euler(60.0f, 0.0f, 0.0f);
		/// 
		/// tween.onUpdate = (t) =>
		/// {
		///		mainCamera.rotation = Quaternion.SlerpUnclamped(startRotation, endRotation, t);
		/// };
		/// 
		/// </code>
		/// </remarks>
		public Action<float> onUpdate;

		/// <summary>Optional, called when the Tween is complete.</summary>
		public Action onComplete;

		/// <summary>
		/// Chains another Tween to this one. Chained Tweens start when this Tween is complete.
		/// </summary>
		public void Chain(Tween chainTween)
		{
			chainTween.enabled = false;

			// add it to the list of chains, creating the list if necessary
			if (chained == null)
			{
				chained = new List<Tween>();
			}

			chained.Add(chainTween);
		}

		#endregion

		#region internals

		private float startTime;                    
		private List<Tween> chained;

		private void Start()
		{
			// Tweens are tracked by comparing startTime to Time.unscaledTime
			startTime = Time.unscaledTime;
		}

		private void Update()
		{
			// calculate the tweening ratio (ie: how complete the tween is)
			var elapsed = Time.unscaledTime - startTime - delay;
			var ratio = elapsed / duration;

			if (ratio >= 1.0f)
			{
				EndTween();
			}
			else if (ratio > 0.0f)
			{
				if(started == false)
				{
					started = true;
					InvokeStart();
				}

				UpdateTween(ratio);
			}
		}

		private void UpdateTween(float ratio)
		{
			InvokeUpdate(Ease(ratio, easing, easingDirection));
		}

		private void EndTween()
		{
			// do a final update to finish the animation neatly
			InvokeUpdate(1.0f);

			// if there are chained Tweens, start them
			if (chained != null)
			{
				foreach (var tween in chained)
				{
					tween.enabled = true;
				}
			}

			// we're all done
			InvokeComplete();
			Destroy(this);
		}

		// Invokers

		private void InvokeStart()
		{
			if (onStart != null)
			{
				onStart.Invoke();
			}
		}

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

		public static float Ease(float value, Easing easing, EasingDirection easingDirection)
		{
			switch (easingDirection)
			{
				case EasingDirection.easeIn:
					// apply formula directly
					return GetEasedValue(value, easing);

				default:
				case EasingDirection.easeOut:
					// apply formula backwards
					return 1.0f - GetEasedValue(1.0f - value, easing);

				case EasingDirection.easeInOut:
					// apply first half forwards and second half backwards
					if (value < 0.5f)
					{
						return GetEasedValue(value * 2.0f, easing) / 2.0f;
					}
					else
					{
						return 1.0f - (GetEasedValue((1.0f - value) * 2.0f, easing) / 2.0f);
					}
			}
		}

		private static float GetEasedValue(float p, Easing easing)
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

		#endregion
	}

	/// <summary>
	/// Adapted from https://easings.net/
	/// </summary>
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
		Bounce
	}

	public enum EasingDirection
	{
		easeIn,
		easeOut,
		easeInOut,
		// shakeItAllAbout,
	}

}
