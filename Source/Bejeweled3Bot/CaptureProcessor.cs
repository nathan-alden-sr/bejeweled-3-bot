using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace NathanAlden.Bejeweled3Bot
{
	public class CaptureProcessor
	{
		public IReadOnlyDictionary<Gem, BitArray2D> Process(Bitmap bitmap, BoardDimensions dimensions)
		{
			Dictionary<Gem, BitArray2D> bitArraysByGem = Gem.All.ToDictionary(arg => arg, arg => new BitArray2D(Constants.DefaultSizeInTiles.Width, Constants.DefaultSizeInTiles.Height));
			BitmapData bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

			try
			{
				for (int tileX = 0; tileX < Constants.DefaultSizeInTiles.Width; tileX++)
				{
					for (int tileY = 0; tileY < Constants.DefaultSizeInTiles.Height; tileY++)
					{
						var pixelX = (int)((tileX * dimensions.TileSize.Width) + ((dimensions.TileSize.Width / 2) - (dimensions.TileSampleSize.Width / 2)));
						var pixelY = (int)((tileY * dimensions.TileSize.Height) + ((dimensions.TileSize.Height / 2) - (dimensions.TileSampleSize.Height / 2)));
						ColorComponents colorComponents = CalculateAverageComponents(bitmapData, pixelX, pixelY, (int)dimensions.TileSampleSize.Width, (int)dimensions.TileSampleSize.Height);

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
				bitmap.UnlockBits(bitmapData);
			}

			return bitArraysByGem;
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