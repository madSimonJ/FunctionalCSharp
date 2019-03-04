using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Functional_CSharp._02_Currying_and_Partial_Application
{
    public class _04___Dynamic_Function
    {
        [Fact]
        public void Test01()
        {
            var customCalcFunc = MakeFunc("add,2|multiply,2");
            customCalcFunc(5).Should().Be(14);
            customCalcFunc(6).Should().Be(16);
        }

        public static IDictionary<string, Func<decimal, Func<decimal, decimal>>> commands =
            new Dictionary<string, Func<decimal, Func<decimal, decimal>>>
            {
                {"add", x => y => x + y},
                {"subtract", x => y => y - x},
                {"multiply", x => y => x * y},
                {"divide", x => y => y / x }
            };

        private Func<decimal, decimal> MakeFunc(string funcString) =>
            input =>
                funcString.Split("|")
                    .Select(x => x.Split(","))
                    .Select(x => commands[x[0]](decimal.Parse(x[1])))
                    .ApplyCalculations(input);
    }

    public static class FuncExtensions
    {
        public static decimal ApplyCalculations(this IEnumerable<Func<decimal, decimal>> @this, decimal currentTotal) =>
            ApplyCalculations(@this.GetEnumerator(), currentTotal);

        public static decimal ApplyCalculations(IEnumerator<Func<decimal, decimal>> enumerator, decimal currentTotal) =>
            enumerator.MoveNext()
                ? ApplyCalculations(enumerator, enumerator.Current(currentTotal))
                : currentTotal;
    }
}
