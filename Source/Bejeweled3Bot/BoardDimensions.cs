﻿using System.Windows;

namespace NathanAlden.Bejeweled3Bot
{
	public class BoardDimensions
	{
		private readonly Rect _clientRectangle;
		private readonly Rect _screenRectangle;
		private readonly Size _tileSampleSize;
		private readonly Size _tileSize;

		public BoardDimensions(Size tileSampleSize, Rect clientRectangle, Rect screenRectangle, Size tileSize)
		{
			_tileSampleSize = tileSampleSize;
			_clientRectangle = clientRectangle;
			_screenRectangle = screenRectangle;
			_tileSize = tileSize;
		}

		public Rect ClientRectangle
		{
			get
			{
				return _clientRectangle;
			}
		}

		public Rect ScreenRectangle
		{
			get
			{
				return _screenRectangle;
			}
		}

		public Size TileSampleSize
		{
			get
			{
				return _tileSampleSize;
			}
		}

		public Size TileSize
		{
			get
			{
				return _tileSize;
			}
		}
	}
}