using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using WindowsInput;

using Point = System.Drawing.Point;
using Size = System.Windows.Size;

namespace NathanAlden.Bejeweled3Bot
{
	public partial class MainWindow
	{
		private static readonly MouseSimulator _mouseSimulator = new MouseSimulator(new InputSimulator());
		private static readonly Size _screenSize;
		private readonly CapturesPerSecondCalculator _capturesPerSecondCalculator = new CapturesPerSecondCalculator();
		private readonly Dictionary<Point, Border> _tilesByPoint = new Dictionary<Point, Border>();
		private readonly DispatcherTimer _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };
		private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

		static MainWindow()
		{
			_screenSize = new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
		}

		public MainWindow()
		{
			InitializeComponent();

			_timer.Tick += (sender, args) => TimerTick();

			GameTypeComboBox.DisplayMemberPath = "Name";
			GameTypeComboBox.ItemsSource = GameType.All.OrderBy(arg => arg.Name);
			GameTypeComboBox.SelectedItem = GameType.Lightning;

			const int tileMargin = 20;

			for (int x = 0; x < Constants.DefaultSizeInTiles.Width; x++)
			{
				for (int y = 0; y < Constants.DefaultSizeInTiles.Height; y++)
				{
					int width = Constants.DefaultTileSize.Width - tileMargin;
					int height = Constants.DefaultTileSize.Height - tileMargin;
					var border = new Border
					{
						Background = new SolidColorBrush(Colors.Black),
						CornerRadius = new CornerRadius(width / 2.0),
						Width = width,
						Height = height,
						Child = new TextBlock
						{
							FontSize = 20,
							FontWeight = FontWeights.Bold,
							HorizontalAlignment = HorizontalAlignment.Center,
							VerticalAlignment = VerticalAlignment.Center
						}
					};

					Canvas.SetLeft(border, (x * Constants.DefaultTileSize.Width) + tileMargin);
					Canvas.SetTop(border, (y * Constants.DefaultTileSize.Height) + tileMargin);

					GameBoardCanvas.Children.Add(border);

					_tilesByPoint.Add(new Point(x, y), border);
				}
			}
		}

		private bool IsCapturing
		{
			get
			{
				return _timer.IsEnabled;
			}
		}

		private void GoButtonClick(object sender, RoutedEventArgs e)
		{
			if (IsCapturing)
			{
				Stop();
			}
			else
			{
				Go();
			}
		}

		private void TimerTick()
		{
			if (ShouldStop())
			{
				Stop();
			}
		}

		private void BoardCapture(Board board, Bitmap bitmap, BoardDimensions dimensions)
		{
			var eventProcessor = new CaptureProcessor();
			IReadOnlyDictionary<Gem, BitArray2D> bitArraysByGem = eventProcessor.Process(bitmap, dimensions);

			foreach (var pair in bitArraysByGem)
			{
				for (int x = 0; x < pair.Value.Width; x++)
				{
					for (int y = 0; y < pair.Value.Height; y++)
					{
						if (pair.Value.Get(x, y))
						{
							Border border = _tilesByPoint[new Point(x, y)];

							border.Background = new SolidColorBrush(pair.Key.BackgroundColor);

							var textBlock = ((TextBlock)border.Child);

							textBlock.Foreground = new SolidColorBrush(pair.Key.ForegroundColor);
							textBlock.Text = pair.Key.Abbreviation;
						}
					}
				}
			}

			Swap[] swaps = Patterns.All
				.SelectMany(
					pattern => bitArraysByGem
						.Where(arg => arg.Key != Gem.Unknown)
						.Select(arg => arg.Value)
						.SelectMany(pattern.GetSwaps))
				.OrderBy(arg => arg.ToTile.Y)
				.ToArray();

			foreach (Swap swap in swaps)
			{
				System.Windows.Point fromCoordinate = board.GetTileCenterCoordinate(swap.FromTile.X, swap.FromTile.Y);
				System.Windows.Point toCoordinate = board.GetTileCenterCoordinate(swap.ToTile.X, swap.ToTile.Y);

				MoveMouseTo(fromCoordinate);
				_mouseSimulator.LeftButtonDown();
				MoveMouseTo(toCoordinate);
				_mouseSimulator.LeftButtonUp();

				if (ShouldStop())
				{
					Stop();
					return;
				}
			}
		}

		private void Stop()
		{
			GoButton.Content = "Go";
			EndCapture();
		}

		private void Go()
		{
			GoButton.Content = "Stop (CTRL+ALT)";
			StartCapture();
		}

		private void StartCapture()
		{
			EndCapture();
			CapturesPerSecondLabel.Content = "";
			_cancellationTokenSource = new CancellationTokenSource();
			_capturesPerSecondCalculator.Restart();

			new Board(App.Bejeweled3Process, (GameType)GameTypeComboBox.SelectedItem)
				.Capture(
					(board, bitmap, dimensions) =>
						Dispatcher.Invoke(() =>
						{
							_capturesPerSecondCalculator.LogCapture();
							CapturesPerSecondLabel.Content = _capturesPerSecondCalculator.CapturesPerSecond.ToString("F1");
							BoardCapture(board, bitmap, dimensions);
						}),
					_cancellationTokenSource.Token);
			_timer.Start();
		}

		private void EndCapture()
		{
			_timer.Stop();
			_capturesPerSecondCalculator.Stop();
			if (_cancellationTokenSource != null)
			{
				_cancellationTokenSource.Cancel();
				_cancellationTokenSource = null;
			}
		}

		private static void MoveMouseTo(System.Windows.Point coordinate)
		{
			_mouseSimulator.MoveMouseTo((coordinate.X / _screenSize.Width) * 65535.0, (coordinate.Y / _screenSize.Height) * 65535.0);
		}

		private static bool ShouldStop()
		{
			return (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt));
		}
	}
}