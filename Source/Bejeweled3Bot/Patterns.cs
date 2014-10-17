using System.Collections.Generic;
using System.Drawing;

namespace NathanAlden.Bejeweled3Bot
{
	public static class Patterns
	{
		public static readonly IEnumerable<Pattern> All;
		public static readonly Pattern HypercubePattern = new Pattern(
			new bool?[,]
			{
				{ true, true, false, true, true },
				{ null, null, true, null, null }
			},
			new Point(2, 1),
			new Point(2, 0));
		public static readonly Pattern SevenPattern = new Pattern(
			new bool?[,]
			{
				{ null, null, true, null, null },
				{ null, null, true, null, null },
				{ true, true, false, true, true },
				{ null, null, true, null, null }
			},
			new Point(2, 3),
			new Point(2, 2));
		public static readonly Pattern SixPattern1 = new Pattern(
			new bool?[,]
			{
				{ null, true, null, null },
				{ null, true, null, null },
				{ true, false, true, true },
				{ null, true, null, null }
			},
			new Point(1, 3),
			new Point(1, 2));
		public static readonly Pattern SixPattern2 = new Pattern(
			new bool?[,]
			{
				{ null, null, true, null },
				{ null, null, true, null },
				{ true, true, false, true },
				{ null, null, true, null }
			},
			new Point(2, 3),
			new Point(2, 2));
		public static readonly Pattern FivePattern1 = new Pattern(
			new bool?[,]
			{
				{ null, true, null, null },
				{ null, true, null, null },
				{ true, false, true, true }
			},
			new Point(0, 2),
			new Point(1, 2));
		public static readonly Pattern FivePattern2 = new Pattern(
			new bool?[,]
			{
				{ null, null, true, null },
				{ null, null, true, null },
				{ true, true, false, true }
			},
			new Point(3, 2),
			new Point(2, 2));
		public static readonly Pattern FivePattern3 = new Pattern(
			new bool?[,]
			{
				{ null, true, null },
				{ null, true, null },
				{ true, false, true },
				{ null, true, null }
			},
			new Point(1, 3),
			new Point(1, 2));
		public static readonly Pattern FourPattern1 = new Pattern(
			new bool?[,]
			{
				{ true, null },
				{ true, null },
				{ false, true },
				{ true, false }
			},
			new Point(1, 2),
			new Point(0, 2));
		public static readonly Pattern FourPattern2 = new Pattern(
			new bool?[,]
			{
				{ true, null },
				{ false, true },
				{ true, null },
				{ true, false }
			},
			new Point(1, 1),
			new Point(0, 1));
		public static readonly Pattern ThreePattern1 = new Pattern(
			new bool?[,]
			{
				{ true, null },
				{ true, null },
				{ false, true }
			},
			new Point(1, 2),
			new Point(0, 2));
		public static readonly Pattern ThreePattern2 = new Pattern(
			new bool?[,]
			{
				{ false, true },
				{ true, null },
				{ true, null }
			},
			new Point(1, 0),
			new Point(0, 0));
		public static readonly Pattern ThreePattern3 = new Pattern(
			new bool?[,]
			{
				{ true },
				{ true },
				{ false },
				{ true }
			},
			new Point(0, 3),
			new Point(0, 2));
		public static readonly Pattern ThreePattern4 = new Pattern(
			new bool?[,]
			{
				{ true, null },
				{ false, true },
				{ true, null }
			},
			new Point(1, 1),
			new Point(0, 1));

		static Patterns()
		{
			var patterns = new List<Pattern>();

			patterns.AddRange(HypercubePattern.GetAllRotations());
			patterns.AddRange(SevenPattern.GetAllRotations());
			patterns.AddRange(SixPattern1.GetAllRotations());
			patterns.AddRange(SixPattern2.GetAllRotations());
			patterns.AddRange(FivePattern1.GetAllRotations());
			patterns.AddRange(FivePattern2.GetAllRotations());
			patterns.AddRange(FivePattern3.GetAllRotations());
			patterns.AddRange(FourPattern1.GetAllRotations());
			patterns.AddRange(FourPattern2.GetAllRotations());
			patterns.AddRange(ThreePattern1.GetAllRotations());
			patterns.AddRange(ThreePattern2.GetAllRotations());
			patterns.AddRange(ThreePattern3.GetAllRotations());
			patterns.AddRange(ThreePattern4.GetAllRotations());

			All = patterns.ToArray();
		}
	}
}