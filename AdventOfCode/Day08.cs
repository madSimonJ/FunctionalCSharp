using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace AdventOfCode.Content
{
    public static class MetaDataCalculator
    {

        public static MetaData ParseMetaData(string rawData) =>
            rawData.SplitOnSpace()
                    .Select(int.Parse)
                    .GetEnumerator()
                    .ParseMetaData();

        private static MetaData ParseMetaData(this IEnumerator<int> rawData) =>
            (
                NumberOfChildNodes: rawData.GetNext(),
                NumberOfMetaDataItems: rawData.GetNext(),
                childNodes: Enumerable.Empty<MetaData>()
            )
            .IfThen(x => x.NumberOfChildNodes > 0, x => (
                                NumberOfChildNodes: x.NumberOfChildNodes,
                                NumberOfMetaDataItems: x.NumberOfMetaDataItems,
                                childNodes: rawData.Repeat(ParseMetaData, x.NumberOfChildNodes).ToArray()
            ))
            .Map(x => new MetaData
            {
                NumberOfChildNodes = x.NumberOfChildNodes,
                NumberOfMetaDataItems = x.NumberOfMetaDataItems,
                Values = rawData.Repeat(y => y.GetNext(), x.NumberOfMetaDataItems).ToArray(),
                Nodes = x.childNodes.ToArray()
            });

        public static int CalculateMetaDataCount(MetaData m) =>
            m.Nodes.Select(x => CalculateMetaDataCount(x)).Sum() + m.Values.Sum();

        public static int CalculateNodeValue(MetaData m) =>
           m.Nodes.Any()
                ? m.Nodes.Select((x, i) => 
                    m.Values.Contains(i + 1)
                        ? CalculateNodeValue(x) * m.Values.Count(y => i + 1 == y)
                        : 0).Sum()
                : m.Values.Sum();
    }

    public class MetaData
    {
        public int NumberOfChildNodes { get; set; }
        public int NumberOfMetaDataItems { get; set; }
        public IEnumerable<int> Values { get; set; }
        public IEnumerable<MetaData> Nodes { get; set; }
    }

    public class Day08
    {
        [Fact]
        public void Day07a_Test_a()
        {
            const string input = "0 3 1 2 3";
            var parsedMetaData = MetaDataCalculator.ParseMetaData(input);
            var output = MetaDataCalculator.CalculateMetaDataCount(parsedMetaData);

            output.Should().Be(6);
        }

        [Fact]
        public void Day07a_Test_b()
        {
            const string input = "1 3 0 3 4 5 6 1 2 3";
            var parsedMetaData = MetaDataCalculator.ParseMetaData(input);
            var output = MetaDataCalculator.CalculateMetaDataCount(parsedMetaData);
            output.Should().Be(21);
        }

        [Fact]
        public void Day07a_Test_c()
        {
            const string input = "2 3 0 3 10 11 12 1 1 0 1 99 2 1 1 2";
            var parsedMetaData = MetaDataCalculator.ParseMetaData(input);
            var output = MetaDataCalculator.CalculateMetaDataCount(parsedMetaData);
            output.Should().Be(138);
        }

        [Fact]
        public void Day07a()
        {
            var input = File.ReadAllText(".\\Content\\Day08.txt");
            var parsedMetaData = MetaDataCalculator.ParseMetaData(input);
            var output = MetaDataCalculator.CalculateMetaDataCount(parsedMetaData);
            output.Should().Be(46781);
        }

        [Fact]
        public void Day07a_Test_d()
        {
            const string input = "2 3 0 3 10 11 12 1 1 0 1 99 2 1 1 2";
            var parsedMetaData = MetaDataCalculator.ParseMetaData(input);
            var output = MetaDataCalculator.CalculateNodeValue(parsedMetaData);
            output.Should().Be(66);
        }

        [Fact]
        public void Day07b()
        {
            var input = File.ReadAllText(".\\Content\\Day08.txt");
            var parsedMetaData = MetaDataCalculator.ParseMetaData(input);
            var output = MetaDataCalculator.CalculateNodeValue(parsedMetaData);
            output.Should().Be(21405);
        }
    }
}
