using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace NathanAlden.Bejeweled3Bot
{
	public class Board
	{
		private readonly BoardDimensions _dimensions;

		public Board(Process process, GameType gameType)
		{
			Win32.RECT clientRect;
			var clientScreenOrigin = new System.Drawing.Point(0, 0);

			Win32.GetClientRect(process.MainWindowHandle, out clientRect);
			Win32.ClientToScreen(process.MainWindowHandle, ref clientScreenOrigin);
			var clientSize = new Size(clientRect.right - clientRect.left, clientRect.bottom - clientRect.top);
			var clientScale = new Point(clientSize.Width / Constants.DefaultClientSize.Width, clientSize.Height / Constants.DefaultClientSize.Height);
			var gameScreenRectangle = new Rect(new Point(clientScreenOrigin.X, clientScreenOrigin.Y), clientSize);
			var tileSize = new Size(Constants.DefaultTileSize.Width * clientScale.X, Constants.DefaultTileSize.Height * clientScale.Y);
			var clientRectangle = new Rect(
				gameType.DefaultBoardOrigin.X * clientScale.X,
				gameType.DefaultBoardOrigin.Y * clientScale.Y,
				tileSize.Width * Constants.DefaultTileSize.Width,
				tileSize.Height * Constants.DefaultTileSize.Height);
			var screenRectangle = new Rect(new Point(gameScreenRectangle.X + clientRectangle.X, gameScreenRectangle.Y + clientRectangle.Y), clientRectangle.Size);

			_dimensions = new BoardDimensions(clientRectangle, screenRectangle, tileSize);
		}

		public void Capture(Action<Board, Bitmap, BoardDimensions> captureDelegate, CancellationToken cancellationToken)
		{
			Task.Run(
				() =>
				{
					using (var bitmap = new Bitmap((int)_dimensions.ClientRectangle.Width, (int)_dimensions.ClientRectangle.Height, PixelFormat.Format32bppArgb))
					using (Graphics graphics = Graphics.FromImage(bitmap))
					{
						while (!cancellationToken.IsCancellationRequested)
						{
							var point = new System.Drawing.Point((int)_dimensions.ScreenRectangle.X, (int)_dimensions.ScreenRectangle.Y);
							var size = new System.Drawing.Size((int)_dimensions.ScreenRectangle.Width, (int)_dimensions.ScreenRectangle.Height);

							graphics.CopyFromScreen(point, new System.Drawing.Point(0, 0), size, CopyPixelOperation.SourceCopy);

							captureDelegate(this, bitmap, _dimensions);
						}
					}
				},
				cancellationToken);
		}

		public Point GetTileCenterCoordinate(int x, int y)
		{
			return new Point(_dimensions.ScreenRectangle.X + (x * _dimensions.TileSize.Width) + (_dimensions.TileSize.Width / 2), _dimensions.ScreenRectangle.Y + (y * _dimensions.TileSize.Height) + (_dimensions.TileSize.Height / 2));
		}
	}
}