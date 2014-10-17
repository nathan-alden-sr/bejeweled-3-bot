using System.Collections.Generic;
using System.Drawing;

namespace NathanAlden.Bejeweled3Bot
{
	public class Pattern
	{
		private readonly Point _from;
		private readonly int _patternHeight;
		private readonly int _patternWidth;
		private readonly Point _to;
		private readonly bool?[,] _yByXValues;

		public Pattern(bool?[,] yByXValues, Point from, Point to)
		{
			_patternHeight = yByXValues.GetLength(0);
			_patternWidth = yByXValues.GetLength(1);
			_yByXValues = yByXValues;
			_from = @from;
			_to = to;
		}

		public IEnumerable<Swap> GetSwaps(BitArray2D bitArray)
		{
			var points = new List<Swap>();

			for (int bitArrayY = 0; bitArrayY <= bitArray.Height - _patternHeight; bitArrayY++)
			{
				for (int bitArrayX = 0; bitArrayX <= bitArray.Width - _patternWidth; bitArrayX++)
				{
					bool skip = false;

					for (int patternX = 0; patternX < _patternWidth && !skip; patternX++)
					{
						for (int patternY = 0; patternY < _patternHeight && !skip; patternY++)
						{
							bool? patternValue = _yByXValues[patternY, patternX];

							if (patternValue == null)
							{
								continue;
							}

							if (bitArray.Get(bitArrayX + patternX, bitArrayY + patternY) != _yByXValues[patternY, patternX])
							{
								skip = true;
							}
						}
					}

					if (!skip)
					{
						points.Add(new Swap(new Point(bitArrayX + _from.X, bitArrayY + _from.Y), new Point(bitArrayX + _to.X, bitArrayY + _to.Y)));
					}
				}
			}

			return points;
		}

		public IEnumerable<Pattern> GetAllRotations()
		{
			yield return this;

			Pattern pattern = this;

			for (int i = 0; i < 3; i++)
			{
				var xByYValues = new bool?[pattern._patternWidth, pattern._patternHeight];

				for (int x = 0; x < pattern._patternHeight; x++)
				{
					for (int y = 0; y < pattern._patternWidth; y++)
					{
						xByYValues[y, x] = pattern._yByXValues[pattern._patternHeight - x - 1, y];
					}
				}

				pattern = new Pattern(xByYValues, new Point(pattern._patternHeight - pattern._from.Y - 1, pattern._from.X), new Point(pattern._patternHeight - pattern._to.Y - 1, pattern._to.X));

				yield return pattern;
			}
		}
	}
}