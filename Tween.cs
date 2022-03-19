using UnityEngine;


namespace Default.Tweening
{
	public class Tween : MonoBehaviour
	{
		private float startTime;                    // tweens are tracked by comparing startTime to Time.unscaledTime

		// public values
		public float duration = 1.0f;               // duration is in seconds
		public Easing easing = Easing.Linear;
		public EasingType type = EasingType.easeIn;

		// output actions
		public System.Action<float> onUpdate;       // called every update with a value between 0.0 and 1.0
		public System.Action onComplete;            // called when tween is complete


		//	*	each tween is attached to an existing GameObject and uses that object's Update phase
		//	*	if that object is destroyed then no more updates will be called 
		//	*	the tween destroys itself when it is complete

		// Constructors

		public static Tween Create(GameObject owner)
		{
			var tween = owner.AddComponent<Tween>();
			tween.startTime = Time.unscaledTime;
			return tween;
		}

		public static Tween Create(Component component)
		{
			// convenience function
			return Create(component.gameObject);
		}

		public void Update()
		{
			// returns true if still running
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

			switch (type)
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
					return Mathf.Sin((p - 1.0f) * Mathf.PI) + 1.0f;

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