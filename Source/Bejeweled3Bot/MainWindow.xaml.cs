using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using WindowsInput;

using Point = System.Drawing.Point;

namespace NathanAlden.Bejeweled3Bot
{
	public partial class MainWindow
	{
		private static readonly MouseSimulator _mouseSimulator = new MouseSimulator(new InputSimulator());
		private static readonly Size _screenSize;
		private readonly Dictionary<Point, Border> _tilesByPoint = new Dictionary<Point, Border>();
		private readonly DispatcherTimer _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(100) };

		static MainWindow()
		{
			_screenSize = new Size(SystemParameters.PrimaryScreenWidth, SystemParameters.PrimaryScreenHeight);
		}

		public MainWindow()
		{
			InitializeComponent();

			_timer.Tick += (sender, args) => TimerStep();

			GameTypeComboBox.DisplayMemberPath = "Name";
			GameTypeComboBox.ItemsSource = GameType.All.OrderBy(arg => arg.Name);
			GameTypeComboBox.SelectedItem = GameType.Lightning;

			const int tileMargin = 20;

			for (int x = 0; x < Board.DefaultSizeInTiles.Width; x++)
			{
				for (int y = 0; y < Board.DefaultSizeInTiles.Height; y++)
				{
					int width = Board.DefaultTileSize.Width - tileMargin;
					int height = Board.DefaultTileSize.Height - tileMargin;
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

					Canvas.SetLeft(border, (x * Board.DefaultTileSize.Width) + tileMargin);
					Canvas.SetTop(border, (y * Board.DefaultTileSize.Height) + tileMargin);

					GameBoardCanvas.Children.Add(border);

					_tilesByPoint.Add(new Point(x, y), border);
				}
			}
		}

		private void DelayIntegerUpDownValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			_timer.Interval = TimeSpan.FromMilliseconds(DelayIntegerUpDown.Value.Value);
		}

		private void SwapZeroButtonClick(object sender, RoutedEventArgs e)
		{
			Step(0);
		}

		private void SwapOneButtonClick(object sender, RoutedEventArgs e)
		{
			Step(1);
		}

		private void StepButtonClick(object sender, RoutedEventArgs e)
		{
			Step();
		}

		private void GoButtonClick(object sender, RoutedEventArgs e)
		{
			if (_timer.IsEnabled)
			{
				Stop();
			}
			else
			{
				Go();
			}
		}

		private void TimerStep()
		{
			if (ShouldStop())
			{
				Stop();
				return;
			}
			if (Win32.GetForegroundWindow() == App.Bejeweled3Process.MainWindowHandle)
			{
				Step();
			}
		}

		private void Step(int? maximumSwaps = null)
		{
			using (var board = new Board(App.Bejeweled3Process, (GameType)GameTypeComboBox.SelectedItem))
			{
				IReadOnlyDictionary<Gem, BitArray2D> bitArraysByGem = board.ProcessGameBoard();

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

				if (maximumSwaps <= 0)
				{
					return;
				}

				int swapCount = 0;
				Swap[] swaps = Patterns.All.SelectMany(pattern => bitArraysByGem.Where(arg => arg.Key != Gem.Unknown).Select(arg => arg.Value).SelectMany(pattern.GetSwaps)).OrderBy(arg => arg.ToTile.Y).ToArray();

				for (int i = 0; i < swaps.Length; i++)
				{
					Swap swap = swaps[i];
					System.Windows.Point fromCoordinate = board.GetTileCenterCoordinate(swap.FromTile.X, swap.FromTile.Y);
					System.Windows.Point toCoordinate = board.GetTileCenterCoordinate(swap.ToTile.X, swap.ToTile.Y);

					MoveMouseTo(fromCoordinate);
					_mouseSimulator.LeftButtonDown();
					MoveMouseTo(toCoordinate);
					_mouseSimulator.LeftButtonUp();

					if (++swapCount == maximumSwaps)
					{
						return;
					}
					if (ShouldStop())
					{
						Stop();
						return;
					}
					if (i < swaps.Length - 1)
					{
						Thread.Sleep(_timer.Interval);
					}
				}
			}
		}

		private void Stop()
		{
			_timer.Stop();
			GoButton.Content = "Go";
		}

		private void Go()
		{
			GoButton.Content = "Stop (CTRL+ALT)";
			_timer.Start();
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