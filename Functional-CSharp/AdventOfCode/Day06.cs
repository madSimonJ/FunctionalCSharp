using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace AdventOfCode
{
    public static class AreaCalculator
    {
        public static IEnumerable<(int X, int Y)> Parse(this IEnumerable<string> input) =>
            input.Select(x => x.Split(","))
                .Select(x => (int.Parse(x[0]), int.Parse(x[1])));

        public static IEnumerable<IEnumerable<int>> DrawGrid(IEnumerable<(int X, int Y)> coords) =>
            coords.Fork<IEnumerable<(int X, int Y)>, int, IEnumerable<IEnumerable<int>>>(
                x => x.ToArray().Map(y => new int[y[0]+1][].Select(z => new int[y[1]+1])),
                x => x.Max(y => y.X),
                x => x.Max(y => y.Y)
                );

        private static int ManhattanDistance(int x1, int y1, int x2, int y2) =>
            Math.Abs(x2 - x1) + Math.Abs(y2 - y1);

        private static int CalculateNearestNeighbour((int Index, int Distance)[] distances) =>
            distances.Length > 1
                ? -2
                : distances.Single().Map(x => x.Distance == 0 ? -1 : x.Index);

        private static int PlotNearestNeighbour(int gridX, int gridY, IEnumerable<(int X, int Y)> Coords) =>
            Coords.Select((x, i) => (Index: i, Distance: ManhattanDistance(gridX, gridY, x.X, x.Y))).ToArray()
                .Map(x => (Array: x, Min: x.Min(y => y.Distance)))
                .Map(x => x.Array.Where((y => y.Distance == x.Min)))
                .ToArray()
                .Map(CalculateNearestNeighbour);

        public static IEnumerable<IEnumerable<int>> PlotNearestNeighbour(this (IEnumerable<(int X, int Y)> Coords, IEnumerable<IEnumerable<int>> Grid) @this) =>
            @this.Grid.Select((x, i) => x.Select((y, j) => PlotNearestNeighbour(i, j, @this.Coords)));

        private static int CalculateSumOfDistances(int gridX, int gridY, IEnumerable<(int X, int Y)> Coords) =>
            Coords.Sum(x => ManhattanDistance(gridX, gridY, x.X, x.Y));

        public static IEnumerable<IEnumerable<int>> PlotSumOfDistances(this (IEnumerable<(int X, int Y)> Coords, IEnumerable<IEnumerable<int>> Grid) @this) =>
            @this.Grid.Select((x, i) =>
            {
                return x.Select((y, j) => CalculateSumOfDistances(i, j, @this.Coords));
            });

        public static IEnumerable<int> GetInfiniteGroups(IEnumerable<IEnumerable<int>> grid) =>
            grid.ToArray().Map(x => new[]
            {
                x.First(),
                x.Last(),
                x.Select(y => y.First()),
                x.Select(y => y.Last())
            })
                .SelectMany(x => x)
                .GroupBy(x => x)
                .Select(x => x.Key)
                .Where(x => x >= 0);

        public static int GetSizeOfLargestArea(string input) =>
            input.SplitOnNewline()
                .Parse().ToArray()
                .Map(x => (
                    Coords: x,
                    Grid: DrawGrid(x)
                ))
                .PlotNearestNeighbour().Select(x => x.ToArray()).ToArray()
                .Map(x => (Graph: x, InfiniteSets: GetInfiniteGroups(x)))
                .Map(x => x.Graph.SelectMany(y => y).Where(y => !x.InfiniteSets.Contains(y) && y >= 0))
                .GroupBy(x => x)
                .Max(x => x.Count() + 1);

        public static object GetSizeOfAreaSurroundedBy(string input, int distance) =>
            input.SplitOnNewline()
                .Parse().ToArray()
                .Map(x => (
                    Coords: x,
                    Grid: DrawGrid(x)
                ))
                .PlotSumOfDistances()
                .SelectMany(x => x)
                .Count(x => x < distance);




    }

    public class Day06
    {
        [Fact]
        public void Day06a_Test()
        {
            var input = "1, 1\r\n1, 6\r\n8, 3\r\n3, 4\r\n5, 5\r\n8, 9";
            var output = AreaCalculator.GetSizeOfLargestArea(input);
            output.Should().Be(17);
        }

        [Fact]
        public void Day06a()
        {
            var input = File.ReadAllText(".\\Content\\Day06.txt");
            var output = AreaCalculator.GetSizeOfLargestArea(input);
            output.Should().Be(4398);
        }

        [Fact]
        public void Day06b_Test()
        {
            var input = "1, 1\r\n1, 6\r\n8, 3\r\n3, 4\r\n5, 5\r\n8, 9";
            var output = AreaCalculator.GetSizeOfAreaSurroundedBy(input, 32);
            output.Should().Be(16);
        }

        [Fact]
        public void Day06b()
        {
            var input = File.ReadAllText(".\\Content\\Day06.txt");
            var output = AreaCalculator.GetSizeOfAreaSurroundedBy(input, 10000);
            output.Should().Be(39560);
        }
    }
}
