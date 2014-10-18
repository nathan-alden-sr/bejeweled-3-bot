using System.Collections.Generic;

namespace NathanAlden.Bejeweled3Bot
{
	public class ProcessedCapture
	{
		private readonly IReadOnlyDictionary<Gem, BitArray2D> _bitArraysByGem;
		private readonly IReadOnlyDictionary<GemType, BitArray2D> _bitArraysByGemType;

		public ProcessedCapture(IReadOnlyDictionary<Gem, BitArray2D> bitArraysByGem, IReadOnlyDictionary<GemType, BitArray2D> bitArraysByGemType)
		{
			_bitArraysByGem = bitArraysByGem;
			_bitArraysByGemType = bitArraysByGemType;
		}

		public IReadOnlyDictionary<Gem, BitArray2D> BitArraysByGem
		{
			get
			{
				return _bitArraysByGem;
			}
		}

		public IReadOnlyDictionary<GemType, BitArray2D> BitArraysByGemType
		{
			get
			{
				return _bitArraysByGemType;
			}
		}
	}
}