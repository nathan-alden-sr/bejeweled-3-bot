using System.Drawing;

namespace NathanAlden.Bejeweled3Bot
{
	public class Swap
	{
		private readonly Point _fromTile;
		private readonly Point _toTile;

		public Swap(Point fromTile, Point toTile)
		{
			_fromTile = fromTile;
			_toTile = toTile;
		}

		public Point FromTile
		{
			get
			{
				return _fromTile;
			}
		}

		public Point ToTile
		{
			get
			{
				return _toTile;
			}
		}
	}
}