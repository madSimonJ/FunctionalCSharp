using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace AdventOfCode
{
    public static class FrequencyCalculator
    {
        private static readonly IDictionary<char, Func<int, Func<int, int>>> Operations = new Dictionary<char, Func<int, Func<int, int>>>
        {
            {'+', x => y => x + y },
            {'-', x => y => y - x }
        };

        private static (char opChar, int opVal) GetOperationValues(string x) =>
            (x.First(), int.Parse(new string(x.Skip(1).ToArray())));

        public static int CalculateFinalFrequency(string input) =>
           input.ToOperations()
                .AggregateFunc();

        private static IEnumerable<Func<int, int>> ToOperations(this string input) =>
            input.SplitOnNewline()
                .Select(GetOperationValues)
                .Select(x => Operations[x.opChar](x.opVal));

        private static ISet<int> UpdateSet(this IEnumerable<int> @this, int newValue) =>
            new HashSet<int>(@this.Concat(new [] {newValue}));

        private static (int? result, ISet<int> progress, int? lastValue) ProcessNext(this int @this, ICollection<int> progress, Func<int, (int? result, ISet<int> progress, int? lastValue)> cont) =>
            progress.Contains(@this)
                ? (@this, null, null)
                : cont(@this);

        public static (int? result, ISet<int> progress, int? lastValue) GetDuplicate(this IEnumerator<Func<int, int>> @this, ISet<int> progress, int lastValue = 0) =>
            @this.MoveNext()
                ? @this.Current(lastValue)
                    .ProcessNext(progress, x => GetDuplicate(@this, progress.UpdateSet(x), x))
                : (null, progress, lastValue);

        public static (int? result, ISet<int> process, int? lastValue) IterateUntilAnswerFound(this IEnumerable<Func<int, int>> @this, (int? result, ISet<int> progress, int? lastValue) input) =>
            input.result.HasValue
                ? input
                : IterateUntilAnswerFound(@this, GetDuplicate(@this.GetEnumerator(), input.progress, input.lastValue ?? 0));


        public static int GetDuplicate(this IEnumerable<Func<int, int>> @this) =>
            @this.IterateUntilAnswerFound((null, new HashSet<int>(new [] {0}), null)).result.Value;
            
        public static int GetDuplicateFrequency(string input) =>
            input.ToOperations()
                .GetDuplicate();
    }

    public class Day01
    {
        [Theory]
        [InlineData("+1\r\n+1\r\n+1", 3)]
        [InlineData("+1\r\n+1\r\n-2", 0)]
        [InlineData("-1\r\n-2\r\n-3", -6)]
        public void Test1(string input, int expectedAnswer)
        {
            var answer = FrequencyCalculator.CalculateFinalFrequency(input);
            answer.Should().Be(expectedAnswer);
        }

        [Theory]
        [InlineData("+1\r\n-1", 0)]
        [InlineData("+3\r\n+3\r\n+4\r\n-2\r\n-4", 10)]
        [InlineData("-6\r\n+3\r\n+8\r\n+5\r\n-6", 5)]
        [InlineData("+7\r\n+7\r\n-2\r\n-7\r\n-4", 14)]
        public void Test2(string input, int expectedAnswer)
        {
            var answer = FrequencyCalculator.GetDuplicateFrequency(input);
            answer.Should().Be(expectedAnswer);
        }

        [Fact]
        public void DayOne_A()
        {
            var input = File.ReadAllText(".\\Content\\Day01.txt");
            var answer = FrequencyCalculator.CalculateFinalFrequency(input);
            answer.Should().Be(437);
        }

        [Fact]
        public void DayOne_B()
        {
            var input = File.ReadAllText(".\\Content\\Day01.txt");
            var answer = FrequencyCalculator.GetDuplicateFrequency(input);
            answer.Should().Be(655);
        }
    }
}
