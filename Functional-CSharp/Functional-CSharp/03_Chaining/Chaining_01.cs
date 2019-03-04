using System;
using FluentAssertions;
using Xunit;

namespace Functional_CSharp._03_Chaining
{
    public class Chaining_01
    {
        public static string FahrenheitToCelsius(decimal temperatureInFahrenheit)
        {
            var returnValue = temperatureInFahrenheit - 32;
            returnValue *= 5;
            returnValue /= 9;
            returnValue = Math.Round(returnValue, 2);
            return $"{returnValue}°C";
        }

        public static string CelsiusToFahrenheit(decimal temperatureInCelsius)
        {
            var returnValue = temperatureInCelsius * 9;
            returnValue /= 5;
            returnValue += 32;
            returnValue = Math.Round(returnValue, 2);
            return $"{returnValue}°F";
        }

        [Fact]
        public void correct_conversion_of_F_to_C() =>
            FahrenheitToCelsius(100).Should().Be("37.78°C");

        [Fact]
        public void correct_conversion_of_C_to_F() =>
            CelsiusToFahrenheit(100).Should().Be("212°F");
    }
}
