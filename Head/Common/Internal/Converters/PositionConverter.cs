using System;
using FileHelpers;

namespace Head.Common.Internal.Converters
{
	class PositionConverter : ConverterBase 
	{ 
		public override object StringToField(string from) 
		{ 
			int position;
			if(Int32.TryParse(from, out position))
				return position;
			return 0;
		} 

		public override string FieldToString(object fieldValue) 
		{ 
			if(fieldValue is int)
			{
				int val = (int)fieldValue;
				return val == 0 ? "Y" : val.ToString();
			}
			return String.Empty;
		} 	     
	} 
}

