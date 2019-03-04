using System;
using FluentAssertions;
using Xunit;

namespace Functional_CSharp._02_Currying
{
    public class Currying_01
    {
        private static Func<int, int> _add(int x) => y => x + y;
        private static Func<int, int> _subtract(int x) => y => y - x;
        private static Func<int, int> _multiply(int x) => y => x * y;
        private static Func<int, int> _divide(int x) => y => y / x;

        [Fact]
        public void Curried_add()
        {
            var add10 = _add(10);
            add10(10).Should().Be(20);
        }

        [Fact]
        public void Curried_subtract()
        {
            var subtract10 = _subtract(10);
            subtract10(20).Should().Be(10);
        }

        [Fact]
        public void Curried_multiply()
        {
            var multiply10 = _multiply(10);
            multiply10(10).Should().Be(100);
        }

        [Fact]
        public void Curried_divide()
        {
            var divide10 = _divide(10);

            divide10(100).Should().Be(10);
        }
    }
}
