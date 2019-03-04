using System;
using FluentAssertions;
using Xunit;

namespace Functional_CSharp._03_Chaining
{
    public class Chaining_03
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
            input.ToIdentity().Bind(_subtract(32))
                .Bind(_multiply(5))
                .Bind(_divide(9))
                .Bind(x => Math.Round(x, 2))
                .Bind(x => $"{x}°C");

        private static string CelsiusToFahrenheit(decimal input) =>
            input.ToIdentity().Bind(_multiply(9))
                .Bind(_divide(5))
                .Bind(_add(32))
                .Bind(x => Math.Round(x, 2))
                .Bind(x => $"{x}°F");
    }

    public class Identity<T>
    {
        public T Value { get; }

        public Identity(T value)
        {
            Value = value;
        }

        public static implicit operator Identity<T> (T @this) => @this.ToIdentity();
        public static implicit operator T(Identity<T> @this) => @this.Value;
    }

    public static class FunctionalExtensions2
    {
        public static Identity<T> ToIdentity<T>(this T @this) => new Identity<T>(@this);

        public static Identity<TToType> Bind<TFromType, TToType>(this Identity<TFromType> @this,
            Func<TFromType, TToType> f) =>
            f(@this.Value).ToIdentity();
    }
}
