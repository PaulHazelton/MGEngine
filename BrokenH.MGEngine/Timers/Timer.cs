using System;
using Microsoft.Xna.Framework;

namespace BrokenH.MGEngine.Timers
{
	public enum TimerType
	{
		LoopOnce,
		LoopCount,
		LoopUntil,
		LoopForever,
	}

	public class Timer
	{
		private double _elapsedTime;

		public readonly double Delay;
		public readonly Action Function;
		public readonly TimerType TimerType = TimerType.LoopOnce;
		public readonly int LoopCount;
		public readonly Func<bool>? LoopCondition;

		public int IterationsComplete { get; private set; } = 0;
		public bool IsComplete { get; private set; } = false;


		public static Timer CreateOnceTimer(double delay, Action function)
			=> new Timer(delay, function, TimerType.LoopOnce);
		public static Timer CreateCountTimer(double delay, Action function, int loopCount)
			=> new Timer(delay, function, TimerType.LoopCount, loopCount: loopCount);
		public static Timer CreateUntilTimer(double delay, Action function, Func<bool> loopCondition)
			=> new Timer(delay, function, TimerType.LoopUntil, loopCondition: loopCondition);
		public static Timer CreateForeverTimer(double delay, Action function)
			=> new Timer(delay, function, TimerType.LoopForever);

		private Timer(double delay, Action function, TimerType timerType = TimerType.LoopOnce, int loopCount = 1, Func<bool>? loopCondition = null)
		{
			Delay = delay;
			Function = function;
			TimerType = timerType;

			LoopCount = loopCount;
			LoopCondition = loopCondition;
		}

		public void Update(GameTime deltaTime)
		{
			_elapsedTime += deltaTime.ElapsedGameTime.TotalSeconds;

			if (TimerType == TimerType.LoopUntil && (LoopCondition?.Invoke() ?? false))
				IsComplete = true;

			if (_elapsedTime >= Delay)
			{
				Function();
				_elapsedTime -= Delay;
				IterationsComplete++;

				if (TimerType == TimerType.LoopOnce || (TimerType == TimerType.LoopCount && IterationsComplete >= LoopCount))
					IsComplete = true;
			}
		}
	}
}