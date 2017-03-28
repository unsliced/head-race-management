using System;
using FileHelpers;

namespace Head.Common.Internal.Converters
{
	class PointsConverter : ConverterBase 
	{ 
		public override object StringToField(string from) 
		{ 
			int sub = 0;
			if(from.StartsWith("R")) 
				sub = 1;
			int position;
			if(Int32.TryParse(from.Substring(sub), out position))
				return position;
			return 0;
		} 

		public override string FieldToString(object fieldValue) 
		{ 
			return fieldValue.ToString();
		} 	     
	}
    class YearsConverter : ConverterBase
    {
        public override object StringToField(string from)
        {
            int sub = 0;
            if (from.StartsWith("U"))
                sub = 1;
            int position;
            if (Int32.TryParse(from.Substring(sub), out position))
                return position;
            return 0;
        }

        public override string FieldToString(object fieldValue)
        {
            return fieldValue.ToString();
        }
    }
}
