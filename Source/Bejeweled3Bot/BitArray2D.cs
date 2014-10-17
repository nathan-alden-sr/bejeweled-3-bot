using System.Collections;

namespace NathanAlden.Bejeweled3Bot
{
	public class BitArray2D
	{
		private readonly BitArray _bitArray;
		private readonly int _height;
		private readonly int _width;

		public BitArray2D(int width, int height)
		{
			_width = width;
			_height = height;
			_bitArray = new BitArray(width * height);
		}

		public int Width
		{
			get
			{
				return _width;
			}
		}

		public int Height
		{
			get
			{
				return _height;
			}
		}

		public bool Get(int x, int y)
		{
			return _bitArray.Get((y * _width) + x);
		}

		public void Set(int x, int y, bool value)
		{
			_bitArray.Set((y * _width) + x, value);
		}

		public void SetAll(bool value)
		{
			_bitArray.SetAll(value);
		}
	}
}