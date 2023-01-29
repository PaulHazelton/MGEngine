using System;
using Microsoft.Xna.Framework;

namespace BrokenH.MGEngine.Cameras
{
	public class CameraShaker
	{
		private class Shaker
		{
			private Random Generator;
			private float _amplitude;
			private float _frequency;
			private float _duration;
			private float _phase;
			private float _elapsedTime;

			public float Offset { get; private set; }

			public Shaker(float amplitude, float duration, float frequency)
			{
				Generator = new();
				_amplitude = amplitude;
				_duration = duration;
				_frequency = frequency;
			}
			public Shaker(float amplitude, float duration, float frequency, float phase)
			: this(amplitude, duration, frequency)
			{
				_phase = phase;
			}
			public Shaker(float amplitude, float duration, float frequency, float phase, float frequencyTolerance, float phaseTolerance)
			: this(amplitude, duration, frequency, phase)
			{
				_frequency *= MathHelper.Lerp(1 - frequencyTolerance, 1 + frequencyTolerance, Generator.NextSingle());
				_phase += MathHelper.Lerp(1 - phaseTolerance, 1 + phaseTolerance, Generator.NextSingle());
			}

			public void Update(float elapsedTime)
			{
				if (_duration == 0)
					return;
				_elapsedTime += elapsedTime;
				if (_elapsedTime > _duration)
				{
					Offset = 0;
					_duration = 0;
					return;
				}

				double delta = Math.Sin((_phase + _frequency * _elapsedTime) * 6.28d);
				double scale = Math.Pow((_elapsedTime / _duration) - 1, 2);
				Offset = (float)(delta * scale * _amplitude);
			}
		}
		public enum ShakePreset
		{
			Small, Medium, Large, ExtraLarge
		}

		public Vector2 PositionOffset { get; private set; }
		public float RotationOffset { get; private set; }

		private Vector2? _impulseDirection;

		private Random _generator;
		private Shaker _x;
		private Shaker _y;
		private Shaker _r;

		public CameraShaker()
		{
			_generator = new();
			_x = new(0, 0, 0);
			_y = new(0, 0, 0);
			_r = new(0, 0, 0);
		}

		public void Shake(ShakePreset preset)
		{
			switch (preset)
			{
				case ShakePreset.ExtraLarge:	Shake(6.0f, 0.010f, 1.50f, 4f); break;
				case ShakePreset.Large:			Shake(4.0f, 0.003f, 0.60f, 5f); break;
				case ShakePreset.Medium:		Shake(1.5f, 0.000f, 0.50f, 7f); break;
				case ShakePreset.Small:			Shake(1.0f, 0.000f, 0.25f, 8f); break;
			}
		}
		public void Shake(float linearAmplitude, float angularAmplitude, float duration, float frequency)
		{
			LinearShake(linearAmplitude, duration, frequency);
			AngularShake(angularAmplitude, duration, frequency);
		}

		public void LinearShake(float amplitude, float duration, float frequency, Vector2? impulseDirection = null)
		{
			PositionOffset = Vector2.Zero;
			_impulseDirection = impulseDirection;

			if (_impulseDirection.HasValue)
			{
				_impulseDirection = Vector2.Normalize(_impulseDirection.Value);
				_x = new(amplitude, duration, frequency, 0, 0.2f, 0);
			}
			else
			{
				bool b = _generator.NextSingle() < 0.5f;
				_x = new(amplitude, duration, frequency * (b ? 0.65f : 1), 0, 0f, 0f);
				_y = new(amplitude, duration, frequency * (b ? 1 : 0.65f), 0, 0f, 0f);
			}
		}

		public void AngularShake(float amplitude, float duration, float frequency, bool? clockwise = null)
		{
			RotationOffset = 0;

			if (clockwise.HasValue)
				_r = new(amplitude, duration, frequency, (clockwise.Value ? 0.0f : 0.5f), 0.5f, 0f);
			else
				_r = new(amplitude, duration, frequency, (_generator.NextSingle() < 0.5f ? 0.0f : 0.5f), 0.5f, 0f);
		}

		public void Update(GameTime deltaTime)
		{
			float seconds = (float)deltaTime.ElapsedGameTime.TotalSeconds;
			_x.Update(seconds);
			_y.Update(seconds);
			_r.Update(seconds);

			if (_impulseDirection.HasValue)
				PositionOffset = _impulseDirection.Value * _x.Offset;
			else
				PositionOffset = new Vector2(_x.Offset, _y.Offset);

			RotationOffset = _r.Offset;
		}
	}
}