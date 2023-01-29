using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace BrokenH.MGEngine.Timers
{
	public class TimerCollection
	{
		private List<Timer> _timers;

		public TimerCollection()
		{
			_timers = new();
		}

		public void AddTimer(Timer timer) => _timers.Add(timer);
		public void AddOnceTimer(double delay, Action function)
			=> AddTimer(Timer.CreateOnceTimer(delay, function));
		public void AddCountTimer(double delay, Action function, int loopCount)
			=> AddTimer(Timer.CreateCountTimer(delay, function, loopCount));
		public void AddUntilTimer(double delay, Action function, Func<bool> loopCondition)
			=> AddTimer(Timer.CreateUntilTimer(delay, function, loopCondition));
		public void AddForeverTimer(double delay, Action function)
			=> AddTimer(Timer.CreateForeverTimer(delay, function));

		public void Update(GameTime deltaTime)
		{
			for (int i = _timers.Count - 1; i >= 0; i--)
			{
				_timers[i].Update(deltaTime);
				if (_timers[i].IsComplete)
					_timers.RemoveAt(i);
			}
		}
	}
}