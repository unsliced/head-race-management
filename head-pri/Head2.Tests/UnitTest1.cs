using System;
using Xunit;
using Head2;

namespace Head2.Tests
{
    public class Head2_Should
    {
        readonly Head2 _head2;

        public Head2_Should()
        {
            _head2 = new Head2();
        }

        [Fact]
        public void Test1()
        {
            var result = _head2.IsPrime(1);

            Assert.False(result, "1 should not be prime");

        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(4)]
        [InlineData(6)]
        public void ReturnFalseGivenValuesLessThan2(int value)
        {
            var result = _head2.IsPrime(value);
            
            Assert.False(result, $"{value} should not be prime");
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(5)]
        [InlineData(7)]
        public void ReturnTrueGivenValuesLessThan10(int value)
        {
            var result = _head2.IsPrime(value);
            
            Assert.True(result, $"{value} should be prime");
        }

        [Fact]
        public void ValidateSalt()
        {
            var result = _head2.Salt;
            Assert.Equal(94, result);
        }
    }
}
