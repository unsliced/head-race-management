using System;

namespace Head2
{
    public class Head2
    {
        public bool IsPrime(int candidate)
        {
            if(candidate <= 1)
                return false;
            if(candidate == 2)
                return true;
            
            throw new NotImplementedException("can only work for values of 2 or lower");
        }
    }
}
