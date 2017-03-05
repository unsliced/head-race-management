using System;
using Xunit;
using Head.Services;

namespace Head.UnitTests.Services
{    
    public class HeadService_SampleFunctionShould
    {
        readonly HeadService _headService;

        public HeadService_SampleFunctionShould()
        {
            _headService = new HeadService();
        }

        [Fact]
        public void ReturnTrueIfGivenPositiveInteger() 
        {
            var result = _headService.SampleFunction(94);
            Assert.True(result, $"94 should be true");
        }
    }
}
