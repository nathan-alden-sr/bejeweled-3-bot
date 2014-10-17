using System.Windows;

namespace NathanAlden.Bejeweled3Bot
{
	public class GameType
	{
		public static readonly GameType[] All;
		public static readonly GameType DiamondMine = new GameType("Diamond Mine", new Point(246, 65));
		public static readonly GameType Lightning = new GameType("Lightning", new Point(261, 65));
		public static readonly GameType Zen = new GameType("Zen", new Point(261, 37));
		private readonly Point _defaultBoardOrigin;
		private readonly string _name;

		static GameType()
		{
			All = new[]
			{
				DiamondMine,
				Lightning,
				Zen
			};
		}

		private GameType(string name, Point defaultBoardOrigin)
		{
			_name = name;
			_defaultBoardOrigin = defaultBoardOrigin;
		}

		public string Name
		{
			get
			{
				return _name;
			}
		}

		public Point DefaultBoardOrigin
		{
			get
			{
				return _defaultBoardOrigin;
			}
		}
	}
}