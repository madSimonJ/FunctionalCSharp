using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace AdventOfCode
{
    public static class FormulaCalculator
    {
        public static string ReactFormula(string input) =>
            Enumerable.Range(65, 26)
                .Map(x =>
                {
                    return x;
                })
                .Select(x => ($"{x.ToLowerChar()}{x.ToUpperChar()}", $"{x.ToUpperChar()}{x.ToLowerChar()}"))
                .Map(x =>
                {
                    return x;

                })
                .Aggregate(input, (acc, pos) =>
                {
                    return acc.Replace(pos.Item1, "").Replace(pos.Item2, "");
                });

        public static string GetReactedFormula(string input, string previousVersion = "") =>
            previousVersion == input
                ? input
                : GetReactedFormula(ReactFormula(input), input);

        private static char ToUpperChar(this int i) =>
            (char) i;

        private static char ToLowerChar(this int i) =>
            char.ToLower((char) i);

        public static int GetShortestFormula(string input) =>
            Enumerable.Range(65, 26)
                .Select(x => ($"{x.ToUpperChar()}", $"{x.ToLowerChar()}"))
                .Select(x => input.Replace(x.Item1, "").Replace(x.Item2, ""))
                .Select(x => GetReactedFormula(x))
                .Min(x => x.Length);
    }

    public class Day05
    {

        [Fact]
        public void Day05a_Test()
        {
            var input = "dabAcCaCBAcCcaDA";
            var output = FormulaCalculator.GetReactedFormula(input);
            output.Should().Be("dabCBAcaDA");
        }

        [Fact]
        public void Day05a()
        {
            var input = File.ReadAllText(".\\Content\\Day05.txt");
            var output = FormulaCalculator.GetReactedFormula(input);
            output.Length.Should().Be(11298);
        }

        [Fact]
        public void Day05b_Test()
        {
            var input = "dabAcCaCBAcCcaDA";
            var output = FormulaCalculator.GetShortestFormula(input);
            output.Should().Be(4);
        }

        [Fact]
        public void Day05b()
        {
            var input = File.ReadAllText(".\\Content\\Day05.txt");
            var output = FormulaCalculator.GetShortestFormula(input);
            output.Should().Be(5148);
        }
    }
}
