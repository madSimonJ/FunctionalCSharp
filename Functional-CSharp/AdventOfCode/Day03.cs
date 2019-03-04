using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AdventOfCode;
using FluentAssertions;
using Xunit;

namespace AdventOfCode_Day03
{
    public static class ClothCalculator
    {

        private static int FromMessyStringToInt(this string @this) =>
            Regex.Replace(@this, @"[^\d]", "")
                .Map(int.Parse);

        private static int GetId(this IReadOnlyList<string> input) =>
            input[0].FromMessyStringToInt();
        private static (int x, int y) GetCoords(this string input, char splitOn) =>
            input.Split(splitOn)
                .Map(x => (x[0].FromMessyStringToInt(), x[1].FromMessyStringToInt()));

        private static (int Id, (int x, int y) startLocation, (int x, int y) size) ParseString(string input) =>
            input.Split(' ')
                .ToArray()
                .Map(x => (x.GetId(), x[2].GetCoords(','), x[3].GetCoords('x')));

        private static IEnumerable<(int Id, (int x, int y) startLocation, (int x, int y) size)> Parse(this string input) =>
            input.SplitOnNewline()
                .Select(ParseString);

        private static IEnumerable<IEnumerable<int>> NewGrid => Enumerable.Repeat(Enumerable.Repeat(0, 1000), 1000);

        private static bool IsInClaim(int x, int y, (int x, int y) startLocation, (int x, int y) size) =>
            x >= startLocation.x && x < (startLocation.x + size.x) &&
            y >= startLocation.y && y < (startLocation.y + size.y);

        private static IEnumerable<IEnumerable<int>> UpdateGrid(IEnumerable<IEnumerable<int>> acc, (int Id, (int x, int y) startLocation, (int x, int y) size) pos) =>
            acc.Select(
                (x, i) => x.Select((y, j) =>
                    IsInClaim(i, j, pos.startLocation, pos.size)
                        ? y + 1
                        : y
                    )
                );

        private static IEnumerable<IEnumerable<int>> ParseToPopulatedGrid(string input) =>
            input.Parse()
                .Aggregate(NewGrid, UpdateGrid);

        private static int FindFreeClaim(this IEnumerable<(int Id, (int x, int y) startLocation, (int x, int y) size)> @this, IEnumerable<IEnumerable<int>> grid) =>
                @this.First(x => grid.Skip(x.startLocation.x).Take(x.size.x).SelectMany(y => y.Skip(x.startLocation.y).Take(x.size.y)).All(z => z == 1)).Id;


        public static int CalculateOverlap(string input) =>
                    ParseToPopulatedGrid(input)
                        .Aggregate(0, (x, y) => x += y.Count(z => z > 1));


        public static int FindFreeClaim(string input) =>
            input.Parse().FindFreeClaim(ParseToPopulatedGrid(input));
    }

    public class Day03
    {

        [Fact]
        public void Day03a_Test()
        {
            var input = "#1 @ 1,3: 4x4\r\n#2 @ 3,1: 4x4\r\n#3 @ 5,5: 2x2";
            var amountOfOverlap = ClothCalculator.CalculateOverlap(input);
            amountOfOverlap.Should().Be(4);
        }

        [Fact]
        public void Day03a()
        {
            var input = File.ReadAllText(".\\Content\\Day03.txt");
            var amountOfOverlap = ClothCalculator.CalculateOverlap(input);
            amountOfOverlap.Should().Be(110389);
        }

        [Fact]
        public void Day03b_Test()
        {
            var input = "#1 @ 1,3: 4x4\r\n#2 @ 3,1: 4x4\r\n#3 @ 5,5: 2x2";
            var freeClaim = ClothCalculator.FindFreeClaim(input);
            freeClaim.Should().Be(3);
        }

        [Fact]
        public void Day03b()
        {
            var input = File.ReadAllText(".\\Content\\Day03.txt");
            var freeClaim = ClothCalculator.FindFreeClaim(input);
            freeClaim.Should().Be(552);
        }
    }
}
