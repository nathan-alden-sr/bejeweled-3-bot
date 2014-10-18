namespace NathanAlden.Bejeweled3Bot
{
	public class GemType
	{
		public static readonly GemType Flame = new GemType("F");
		public static readonly GemType Normal = new GemType("");
		public static readonly GemType Star = new GemType("S");
		public static readonly GemType Supernova = new GemType("N");
		public static readonly GemType Time = new GemType("T");
		private readonly string _abbreviation;

		private GemType(string abbreviation)
		{
			_abbreviation = abbreviation;
		}

		public string Abbreviation
		{
			get
			{
				return _abbreviation;
			}
		}
	}
}