﻿// The MIT License (MIT)
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


	public class TweenMaker : MonoBehaviour
	{
		#region public API

		/// Duration in seconds.
		public float duration = 1.0f;

		/// Delay in seconds.
		public float delay = 0.0f;

		/// Whether to continuously loop the tween.
		public bool loop = false;

		/// Whether to pingPong (ie: play backwards after playing forwards).
		public bool pingPong = false;

		/// Easing direction.
		public EasingDirection easingDirection = EasingDirection.easeOut;

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

		/// <summary>Called when the tween is complete.</summary>
		/// <remarks>Optional</remarks>
		public Action onComplete;

		/// <summary>Static constructor.</summary>
		/// <param name="owner">The Unity GameObject that will host the TweenMaker component.</param>
		/// <returns>The TweenMaker component</returns>
		/// <remarks>
		/// This is the principal constructor for TweenMaker; all TweenMaker examples start here.
		/// 
		/// Example:
		/// <code>
		///		var tween = TweenMaker.Create(gameObject);
		///		tween.duration = 2.0f;
		///		tween.easing = Easing.Back;
		///		tween.easingDirection = EasingDirection.easeInOut;
		/// </code>
		/// 
		/// Although you can use any GameObject to host the component, it's a good idea to use the object
		/// that is being animated because deleting that object will also delete any TweenMaker tweens still running.
		/// 
		/// The component destroys itself when it is complete.
		/// </remarks>
		public static TweenMaker Create(GameObject owner)
		{
			return owner.AddComponent<TweenMaker>();
		}

		/// <summary>
		/// Creates another TweenMaker tween that will start when this tween completes. The new TweenMaker is
		/// hosted by the same GameObject.
		/// </summary>
		/// <returns>The chained TweenMaker component.</returns>
		public TweenMaker CreateChain()
		{
			next = TweenMaker.Create(gameObject);
			next.enabled = false;

			return next;
		}

		/// <summary>
		/// Specify the easing function by type.
		/// </summary>
		/// <remarks>
		/// This is the normal way to specify easing.
		/// </remarks>
		public Easing easing
		{
			set
			{
				easingFunction = GetEasingFunction(value);
			}
		}

		/// <summary>
		/// Specify the easing function explicitly.
		/// </summary>
		/// <param name="function">The easing function.</param>
		/// <remarks>
		/// Example:
		/// <code>
		///		tween.SetCustomEasingFunction((p) => {
		///			return Mathf.Abs(Mathf.Sin(p* Mathf.PI* 6.5f)) * p;
		///		});
		/// </code>
		/// This is an optional advanced feature that should only be used if you want to spend several days fiddling with
		/// math calls.
		/// </remarks>
		public void SetCustomEasingFunction(Func<float, float> function)
		{
			easing = Easing.Custom;
			easingFunction = function;
		}

		#endregion

		#region internals

		private float startTime;                    
		private Func<float, float> easingFunction;
		private TweenMaker next;

		private void Start()
		{
			// tweens are tracked by comparing startTime to Time.unscaledTime
			startTime = Time.unscaledTime;

			// if no easing function specified use Linear
			if(easingFunction == null)
			{
				easingFunction = GetEasingFunction(Easing.Linear);
			}
		}

		private void Update()
		{
			// calculate the tween ratio (ie: how complete the tween is)
			var elapsed = Time.unscaledTime - startTime - delay;
			var ratio = elapsed / duration;

			if (ratio >= 1.0f)
			{
				EndTween();
			}
			else if (ratio > 0.0f)
			{
				UpdateTween(ratio);
			}
		}

		private Func<float, float> GetEasingFunction(Easing easing)
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

		private void UpdateTween(float ratio)
		{
			if (pingPong)
			{
				// pingPong update
				if (ratio < 0.5f)
				{
					// ping
					InvokeUpdate(Ease(ratio * 2.0f));
				}
				else
				{
					// pong
					InvokeUpdate(Ease((1.0f - ratio) * 2.0f));
				}
			}
			else
			{
				// normal update
				InvokeUpdate(Ease(ratio));
			}
		}

		private void EndTween()
		{
			// do a final update to finish the animation neatly
			InvokeUpdate(pingPong ? 0.0f : 1.0f);

			if (loop)
			{
				// go back to start
				startTime = Time.unscaledTime;
			}
			else
			{
				// if there's a chained tween, start that
				if (next != null)
				{
					next.enabled = true;
				}

				// we're all done
				InvokeComplete();
				Destroy(this);
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

				default:
				case EasingDirection.easeOut:
					// apply formula backwards
					return 1.0f - EasedValue(1.0f - value);

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
			return easingFunction.Invoke(p);
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
		Bounce,
		Custom,
	}

	public enum EasingDirection
	{
		easeIn,
		easeOut,
		easeInOut,
		// shakeItAllAbout,
	}

}
