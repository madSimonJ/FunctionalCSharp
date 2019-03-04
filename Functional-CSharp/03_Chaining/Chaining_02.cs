using System;
using FluentAssertions;
using Xunit;

namespace Functional_CSharp._03_Chaining
{
    public class Chaining_02
    {
        public static Func<decimal, Func<decimal, decimal>> _add = x => y => x + y;
        public static Func<decimal, Func<decimal, decimal>> _subtract = x => y => y - x;
        public static Func<decimal, Func<decimal, decimal>> _multiply = x => y => x * y;
        public static Func<decimal, Func<decimal, decimal>> _divide = x => y => y / x;


        [Fact]
        public void correct_conversion_of_F_to_C() =>
            FahrenheitToCelsius(100).Should().Be("37.78°C");

        [Fact]
        public void correct_conversion_of_C_to_F() =>
            CelsiusToFahrenheit(100).Should().Be("212°F");

        private static string FahrenheitToCelsius(decimal input) =>
            input.Map(_subtract(32))
                .Map(_multiply(5))
                .Map(_divide(9))
                .Map(x => Math.Round(x, 2))
                .Map(x => $"{x}°C");

        private static string CelsiusToFahrenheit(decimal input) =>
            input.Map(_multiply(9))
                .Map(_divide(5))
                .Map(_add(32))
                .Map(x => Math.Round(x, 2))
                .Map(x => $"{x}°F");
    }

    public static class FunctionalExtensions
    {
        public static TToType Map<TFromType, TToType>(this TFromType @this, Func<TFromType, TToType> f) =>
            f(@this);
    }
}
