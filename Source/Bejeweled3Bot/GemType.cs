using System.Collections.Generic;

namespace NathanAlden.Bejeweled3Bot
{
	public class GemType
	{
		public static readonly IEnumerable<GemType> All;
		public static readonly GemType Flame = new GemType("F", 4);
		public static readonly GemType Normal = new GemType("", 5);
		public static readonly GemType Star = new GemType("S", 3);
		public static readonly GemType Supernova = new GemType("N", 2);
		public static readonly GemType Time = new GemType("T", 1);
		private readonly string _abbreviation;
		private readonly int _rank;

		static GemType()
		{
			All = new[]
			{
				Flame,
				Normal,
				Star,
				Supernova,
				Time
			};
		}

		private GemType(string abbreviation, int rank)
		{
			_abbreviation = abbreviation;
			_rank = rank;
		}

		public string Abbreviation
		{
			get
			{
				return _abbreviation;
			}
		}

		public int Rank
		{
			get
			{
				return _rank;
			}
		}
	}
}