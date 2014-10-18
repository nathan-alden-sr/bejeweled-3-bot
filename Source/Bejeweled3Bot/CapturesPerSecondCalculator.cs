using System;
using System.Diagnostics;

namespace NathanAlden.Bejeweled3Bot
{
	public class CapturesPerSecondCalculator
	{
		private readonly Stopwatch _stopwatch = new Stopwatch();
		private int _captures;

		public double CapturesPerSecond
		{
			get
			{
				return _stopwatch.Elapsed.TotalSeconds >= Double.Epsilon ? _captures / _stopwatch.Elapsed.TotalSeconds : 0;
			}
		}

		public void Restart()
		{
			_captures = 0;
			_stopwatch.Restart();
		}

		public void Stop()
		{
			_stopwatch.Stop();
		}

		public void LogCapture()
		{
			_captures++;
		}
	}
}