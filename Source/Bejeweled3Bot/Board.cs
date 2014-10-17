using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows;

using Point = System.Windows.Point;
using Size = System.Drawing.Size;

namespace NathanAlden.Bejeweled3Bot
{
	public class Board : IDisposable
	{
		public static readonly Size DefaultSizeInTiles = new Size(8, 8);
		public static readonly Size DefaultTileSize = new Size(64, 64);
		private static readonly Size _defaultTileSampleSize = new Size(15, 15);
		private readonly Bitmap _bitmap;
		private readonly Graphics _graphics;
		private readonly Rect _screenRectangle;
		private readonly System.Windows.Size _tileSampleSize;
		private readonly System.Windows.Size _tileSize;

		public Board(Process process, GameType gameType)
		{
			Win32.RECT clientRect;
			var clientScreenOrigin = new System.Drawing.Point(0, 0);

			Win32.GetClientRect(process.MainWindowHandle, out clientRect);
			Win32.ClientToScreen(process.MainWindowHandle, ref clientScreenOrigin);

			var clientSize = new System.Windows.Size(clientRect.right - clientRect.left, clientRect.bottom - clientRect.top);
			var clientScale = new Point(clientSize.Width / Constants.DefaultClientSize.Width, clientSize.Height / Constants.DefaultClientSize.Height);
			var gameScreenRectangle = new Rect(new Point(clientScreenOrigin.X, clientScreenOrigin.Y), clientSize);

			_tileSize = new System.Windows.Size(DefaultTileSize.Width * clientScale.X, DefaultTileSize.Height * clientScale.Y);
			_tileSampleSize = new System.Windows.Size(_defaultTileSampleSize.Width * clientScale.X, _defaultTileSampleSize.Height * clientScale.Y);

			var gameBoardClientRectangle = new Rect(
				gameType.DefaultBoardOrigin.X * clientScale.X,
				gameType.DefaultBoardOrigin.Y * clientScale.Y,
				_tileSize.Width * DefaultSizeInTiles.Width,
				_tileSize.Height * DefaultSizeInTiles.Height);

			_screenRectangle = new Rect(new Point(gameScreenRectangle.X + gameBoardClientRectangle.X, gameScreenRectangle.Y + gameBoardClientRectangle.Y), gameBoardClientRectangle.Size);
			_bitmap = new Bitmap((int)gameBoardClientRectangle.Width, (int)gameBoardClientRectangle.Height, PixelFormat.Format32bppArgb);
			_graphics = Graphics.FromImage(_bitmap);
		}

		public void Dispose()
		{
			if (_bitmap != null)
			{
				_bitmap.Dispose();
			}
			if (_graphics != null)
			{
				_graphics.Dispose();
			}
		}

		public IReadOnlyDictionary<Gem, BitArray2D> ProcessGameBoard()
		{
			var point = new System.Drawing.Point((int)_screenRectangle.X, (int)_screenRectangle.Y);
			var size = new Size((int)_screenRectangle.Width, (int)_screenRectangle.Height);

			_graphics.CopyFromScreen(point, new System.Drawing.Point(0, 0), size, CopyPixelOperation.SourceCopy);

			Dictionary<Gem, BitArray2D> bitArraysByGem = Gem.All.ToDictionary(arg => arg, arg => new BitArray2D(DefaultSizeInTiles.Width, DefaultSizeInTiles.Height));
			BitmapData bitmapData = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), ImageLockMode.ReadOnly, _bitmap.PixelFormat);

			try
			{
				for (int tileX = 0; tileX < DefaultSizeInTiles.Width; tileX++)
				{
					for (int tileY = 0; tileY < DefaultSizeInTiles.Height; tileY++)
					{
						var pixelX = (int)((tileX * _tileSize.Width) + ((_tileSize.Width / 2) - (_tileSampleSize.Width / 2)));
						var pixelY = (int)((tileY * _tileSize.Height) + ((_tileSize.Height / 2) - (_tileSampleSize.Height / 2)));
						ColorComponents colorComponents = CalculateAverageComponents(bitmapData, pixelX, pixelY, (int)_tileSampleSize.Width, (int)_tileSampleSize.Height);

						if (colorComponents.Saturation <= 0.1)
						{
							bitArraysByGem[Gem.White].Set(tileX, tileY, true);
						}
						else if (colorComponents.Hue >= 35 && colorComponents.Hue < 55)
						{
							bitArraysByGem[Gem.Orange].Set(tileX, tileY, true);
						}
						else if (colorComponents.Hue >= 55 && colorComponents.Hue < 65)
						{
							bitArraysByGem[Gem.Yellow].Set(tileX, tileY, true);
						}
						else if (colorComponents.Hue >= 110 && colorComponents.Hue < 140)
						{
							bitArraysByGem[Gem.Green].Set(tileX, tileY, true);
						}
						else if (colorComponents.Hue >= 200 && colorComponents.Hue < 240)
						{
							bitArraysByGem[Gem.Blue].Set(tileX, tileY, true);
						}
						else if (colorComponents.Hue >= 345 || colorComponents.Hue < 10)
						{
							bitArraysByGem[Gem.Red].Set(tileX, tileY, true);
						}
						else if (colorComponents.Hue >= 295 && colorComponents.Hue < 305)
						{
							bitArraysByGem[Gem.Purple].Set(tileX, tileY, true);
						}
						else
						{
							bitArraysByGem[Gem.Unknown].Set(tileX, tileY, true);
						}
					}
				}
			}
			finally
			{
				_bitmap.UnlockBits(bitmapData);
			}

			return bitArraysByGem;
		}

		public Point GetTileCenterCoordinate(int x, int y)
		{
			return new Point(_screenRectangle.X + (x * _tileSize.Width) + (_tileSize.Width / 2), _screenRectangle.Y + (y * _tileSize.Height) + (_tileSize.Height / 2));
		}

		private static ColorComponents CalculateAverageComponents(BitmapData bitmapData, int x, int y, int width, int height)
		{
			unsafe
			{
				var hues = new List<float>();
				var saturations = new List<float>();
				var pointer = (byte*)bitmapData.Scan0.ToPointer();

				for (int pointerX = x; pointerX < x + width; pointerX++)
				{
					for (int pointerY = y; pointerY < y + height; pointerY++)
					{
						int bytesPerPixel = Image.GetPixelFormatSize(bitmapData.PixelFormat) / 8;
						int pixelIndex = (pointerY * bitmapData.Stride) + (pointerX * bytesPerPixel);
						Color color = Color.FromArgb(pointer[pixelIndex + 2], pointer[pixelIndex + 1], pointer[pixelIndex]);

						hues.Add(color.GetHue());
						saturations.Add(color.GetSaturation());
					}
				}

				return new ColorComponents(hues.Average(), saturations.Average());
			}
		}

		private class ColorComponents
		{
			private readonly double _hue;
			private readonly double _saturation;

			public ColorComponents(double hue, double saturation)
			{
				_hue = hue;
				_saturation = saturation;
			}

			public double Hue
			{
				get
				{
					return _hue;
				}
			}

			public double Saturation
			{
				get
				{
					return _saturation;
				}
			}
		}
	}
}