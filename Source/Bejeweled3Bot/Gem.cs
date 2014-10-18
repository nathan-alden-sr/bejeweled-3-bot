using System.Windows.Media;

namespace NathanAlden.Bejeweled3Bot
{
	public class Gem
	{
		public static readonly Gem[] All;
		public static readonly Gem Blue = new Gem("B", Colors.Blue, Colors.White, 2);
		public static readonly Gem Green = new Gem("G", Colors.Lime, Colors.Black, 2);
		public static readonly Gem Hypercube = new Gem("H", Color.FromArgb(255, 96, 96, 96), Colors.White, 1);
		public static readonly Gem Orange = new Gem("O", Colors.DarkOrange, Colors.Black, 2);
		public static readonly Gem Purple = new Gem("P", Colors.Magenta, Colors.White, 2);
		public static readonly Gem Red = new Gem("R", Colors.Red, Colors.White, 2);
		public static readonly Gem Unknown = new Gem("U", Color.FromArgb(255, 32, 32, 32), Colors.White, null);
		public static readonly Gem White = new Gem("W", Colors.LightGray, Colors.Black, 2);
		public static readonly Gem Yellow = new Gem("Y", Colors.Yellow, Colors.Black, 2);

		private readonly string _abbreviation;
		private readonly Color _backgroundColor;
		private readonly Color _foregroundColor;
		private readonly int? _rank;

		static Gem()
		{
			All = new[]
			{
				Blue,
				Green,
				Hypercube,
				Orange,
				Purple,
				Red,
				Unknown,
				White,
				Yellow
			};
		}

		private Gem(string abbreviation, Color backgroundColor, Color foregroundColor, int? rank)
		{
			_abbreviation = abbreviation;
			_backgroundColor = backgroundColor;
			_foregroundColor = foregroundColor;
			_rank = rank;
		}

		public string Abbreviation
		{
			get
			{
				return _abbreviation;
			}
		}

		public Color BackgroundColor
		{
			get
			{
				return _backgroundColor;
			}
		}

		public Color ForegroundColor
		{
			get
			{
				return _foregroundColor;
			}
		}

		public int? Rank
		{
			get
			{
				return _rank;
			}
		}
	}
}