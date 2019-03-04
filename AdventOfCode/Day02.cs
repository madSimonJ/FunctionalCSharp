using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace AdventOfCode
{
    public static class ChecksumCalculator
    {
        private static bool HasCharGroupCount(this string input, int count) =>
            input.ToCharArray()
                .GroupBy(x => x)
                .Any(x => x.Count() == count);

        public static int CalculateChecksum(string barcode) =>
            barcode.SplitOnNewline()
                .Fork<IEnumerable<string>, int>(x => x.Aggregate((a, b) => a * b),
                    x => x.Count(y => y.HasCharGroupCount(2)),
                    x => x.Count(y => y.HasCharGroupCount(3))
                );

        private static bool IsOneDifferent(string a, string b, int pos = 0, int numDifferent = 0) =>
            pos == a.Length - 1 || numDifferent > 1
                ? numDifferent == 1
                : IsOneDifferent(a, b, pos + 1, a[pos] == b[pos] ? numDifferent : numDifferent + 1);

        private static (string, string) GetSimilarCodes(IEnumerator<string> e, string input) =>
            e.MoveNext()
                ? IsOneDifferent(input, e.Current)
                    ? (input, e.Current)
                    : GetSimilarCodes(e, input)
                : (null, null);

        private static (string, string) IfNull(this (string, string) @this, Func<(string, string)> f) =>
            @this.Item1 == null
                ? f()
                : @this;

        private static (string, string) ReturnFirstNonNull(this IEnumerator<string> @this, IEnumerable<string> input) =>
            @this.MoveNext()
                ? GetSimilarCodes(input.GetEnumerator(), @this.Current)
                    .IfNull(() => ReturnFirstNonNull(@this, input))
                : (null, null);

        public static string GetSimilarCodes(IEnumerable<string> input) =>
            input.GetEnumerator().ReturnFirstNonNull(input)
                .Map(x => new string(x.Item1.Where((y, i) => y == x.Item2[i]).ToArray()));



    }

    public class Day02
    {
        [Fact]
        public void Day2a_Test()
        {
            var input = "abcdef\r\nbababc\r\nabbcde\r\nabcccd\r\naabcdd\r\nabcdee\r\nababab";
            var answer = ChecksumCalculator.CalculateChecksum(input);
            answer.Should().Be(12);
        }

        [Fact]
        public void Day2a()
        {
            var input = File.ReadAllText(".\\Content\\Day02.txt");
            var answer = ChecksumCalculator.CalculateChecksum(input);
            answer.Should().Be(5681);
        }

        [Fact]
        public void Day2b_Test()
        {
            var input = "abcde\r\nfghij\r\nklmno\r\npqrst\r\nfguij\r\naxcye\r\nwvxyz";
            var answer = ChecksumCalculator.GetSimilarCodes(input.SplitOnNewline().ToArray());
            answer.Should().Be("fgij");
        }

        [Fact]
        public void Day2b()
        {
            var input = File.ReadAllText(".\\Content\\Day02.txt");
            var answer = ChecksumCalculator.GetSimilarCodes(input.SplitOnNewline().ToArray());
            answer.Should().Be("uqyoeizfvmbistpkgnocjtwld");
        }
            
    }
}
