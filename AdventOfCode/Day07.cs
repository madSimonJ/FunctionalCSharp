using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using Xunit;

namespace AdventOfCode
{
    public static class InstructionCalculator
    {
        public static char CalculateNextStep((char Step, IEnumerable<char> required)[] groupedSteps, string remainingSteps, string progress) =>
            remainingSteps.First(x =>
            {
                var result = !groupedSteps.Select(y => y.Step).Contains(x) ||
                       groupedSteps.SingleOrDefault(y => y.Step == x).required.All(progress.Contains);
                return result;
            });


        private static string DetermineNextStep(this (char Step, IEnumerable<char> required)[] groupedSteps, string remainingSteps, string progress = "") =>
            remainingSteps == string.Empty
                ? progress
                : CalculateNextStep(groupedSteps, remainingSteps, progress).Map(x =>
                    DetermineNextStep(groupedSteps, remainingSteps.Replace(x.ToString() , ""), progress + x));

        public static object CalculateInstructionOrder(string input) =>
            input.SplitOnNewline()
                .Select(x => x.Split(" "))
                .Select(x => (Required: x[1].Single(), Step: x[7].Single()))
                .ToArray()
                .Map(x => (
                    GroupedSteps: x.GroupBy(y => y.Step).Select(y => (Step: y.Key, Requires: y.Select(z => z.Required))).ToArray(),
                    Range: x.Fork(y => y.ToArray().Map(z => Enumerable.Range(z[0], z[1]-z[0])),
                            y => y.Select(z => z.Step).Min(),
                            y => y.Select(z => z.Step).Max() + 1
                        ).Select(y => (char)y)
                    )
                ).Map(x => x.GroupedSteps.DetermineNextStep(new string(x.Range.ToArray())));

        private static int CalculateTaskEndTime(int timeStarted, char Task) =>
            timeStarted + (Task - 65);

        private static (IEnumerable<(int TimeStarted, char Task)> Workers, IEnumerable<(char Step, IEnumerable<char> Required)> GroupedSteps, string RemainingSteps) Update((IEnumerable<(int TimeStarted, char Task)> Workers, IEnumerable<(char Step, IEnumerable<char> Required)> GroupedSteps, string RemainingSteps) input, int time) =>
            (
                Workers: input.Workers.Select(x => time > CalculateTaskEndTime(x.TimeStarted, x.Task) ? (0, ' ') : x),
                GroupedSteps: input.GroupedSteps,
                RemainingSteps: input.Workers.Where(x => time > CalculateTaskEndTime(x.TimeStarted, x.Task)).Aggregate(input.RemainingSteps, (acc, curr) => acc.Replace(curr.Task.ToString(), ""))
            );
        
        public static int Tick((IEnumerable<(int TimeStarted, char Task)> Workers, IEnumerable<(char Step, IEnumerable<char> Required)> GroupedSteps, string RemainingSteps) input, int time)  =>
            input.RemainingSteps == string.Empty
                ? time
                : input.Map(x => Update(x, time)).Map(x => Tick(x, time + 1));

        public static object CalculateTimeWithWorkers(string input, int noWorkers) =>
            input.SplitOnNewline()
                .Select(x => x.Split(" "))
                .Select(x => (Required: x[1].Single(), Step: x[7].Single()))
                .ToArray()
                .Map(x => (
                    Workers: Enumerable.Range(0, noWorkers).Select(y => (TimeStarted: 0, Task: ' ')),
                    GroupedSteps: x.GroupBy(y => y.Step).Select(y => (Step: y.Key, Requires: y.Select(z => z.Required))).ToArray(),
                    Range: x.Fork(y => y.ToArray().Map(z => Enumerable.Range(z[0], z[1] - z[0])),
                        y => y.Select(z => z.Step).Min(),
                        y => y.Select(z => z.Step).Max() + 1
                    ).Select(y => (char)y)
                ));
    }

    public class Day07
    {

        [Fact]
        public void Day07a_Test()
        {
            var input = new []
            {
                "Step C must be finished before step A can begin.",
                "Step C must be finished before step F can begin.",
                "Step A must be finished before step B can begin.",
                "Step A must be finished before step D can begin.",
                "Step B must be finished before step E can begin.",
                "Step D must be finished before step E can begin.",
                "Step F must be finished before step E can begin."
            };
            var output = InstructionCalculator.CalculateInstructionOrder(string.Join("\r\n", input));
            output.Should().Be("CABDFE");
        }

        [Fact]
        public void Day07a()
        {
            var input = File.ReadAllText(".\\Content\\Day07.txt");
            var output = InstructionCalculator.CalculateInstructionOrder(input);
            output.Should().Be("MNQKRSFWGXPZJCOTVYEBLAHIUD");
        }

        [Fact]
        public void Day07b_Test()
        {
            var input = new[]
            {
                "Step C must be finished before step A can begin.",
                "Step C must be finished before step F can begin.",
                "Step A must be finished before step B can begin.",
                "Step A must be finished before step D can begin.",
                "Step B must be finished before step E can begin.",
                "Step D must be finished before step E can begin.",
                "Step F must be finished before step E can begin."
            };
            var output = InstructionCalculator.CalculateTimeWithWorkers(string.Join("\r\n", input), 2);
            output.Should().Be(5);
        }
    }
}
