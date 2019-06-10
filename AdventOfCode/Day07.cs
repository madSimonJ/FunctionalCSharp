using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using FluentAssertions;
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

        public static string CalculateInstructionOrder(string input) =>
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

        private static IEnumerable<StepWithTime> ToStepWithTime(this IEnumerable<StepWithDependencies> @this, int TimeDelay) =>
            @this.Select(x =>
                new StepWithTime
                {
                    Step = x.Step,
                    Time = x.Step - 65 + TimeDelay,
                    Done = false,
                    DependsOn = x.DependsOn
                }
            );

        private static IEnumerable<Worker> CreateWorkers(this int @this) =>
            Enumerable.Range(0, @this)
                .Select(x => new Worker
                {
                    Id = x,
                    CurrentStep = ' '
                });

        private static bool WorkerNeedsWork(this Worker @this) =>
            @this.CurrentStep == ' ';

        private static Func<StepWithTime> GetNextStepWithTime(this IEnumerable<StepWithTime> @this) =>
            @this.ToArray().GetEnumerator()
                .Map<IEnumerator, Func<StepWithTime>>(
                    x => () => x.MoveNext()
                        ? x.Current as StepWithTime
                        : null as StepWithTime);

        private static State UpdateStateByOneSecond(this State @this) =>
            @this.CompletedSteps.Concat(@this.Workers
                    .Where(x => @this.Time >= x.SecondStepCompletedBy && x.CurrentStep != ' ')
                    .Select(x => x.CurrentStep)).ToArray()
                .Map(x =>
                {
                    return new State
                    {
                        TimeDelay = @this.TimeDelay,
                        CompletedSteps = x,
                        Time = @this.Time + 1,
                        StepsToDo = @this.StepsToDo.Where(y => !x.Contains(y.Step)),
                        Workers = @this.Workers.Select(y => x.Contains(y.CurrentStep)
                            ? new Worker
                            {
                                CurrentStep = ' ',
                                Id = y.Id,
                                SecondStepCompletedBy = 0
                            }
                            : y)
                    };
                })
                .Map(state =>
                {
                    var a = AssignWorkToUsers(state);
                    return a;
                });

        private static IEnumerable<StepWithTime> TasksAvailableToDo(this State @this, IEnumerable<Worker> workers) =>
            @this.StepsToDo
                .Where(x => !workers.Select(y => y.CurrentStep).Contains(x.Step) && x.StepCanBeStarted(@this.CompletedSteps));

        private static Worker AssignTaskToWorker(this Worker @this, StepWithTime task, int currentTime) =>
            new Worker
            {
                Id = @this.Id,
                CurrentStep = task?.Step ?? ' ',
                SecondStepCompletedBy = task?.Time + currentTime ?? 0
            };

        private static State AssignWorkToUsers(this State @this) =>
            new State
            {
                StepsToDo = @this.StepsToDo,
                CompletedSteps = @this.CompletedSteps,
                Time = @this.Time,
                Workers = GetNextStepWithTime(@this.TasksAvailableToDo(@this.Workers))
                    .Map(x => 
                            @this.Workers.Select(y => 
                                    y.WorkerNeedsWork()
                                        ? y.AssignTaskToWorker(x(), @this.Time)
                                        : y
                                ).ToArray()
                        )
            };

        private static State CreateInitialState(string input, int noWorkers, int timeDelay) =>
                new State
                {
                    Workers = noWorkers.CreateWorkers(),
                    StepsToDo = input.ParseInstructionLines().ToStepWithTime(timeDelay),
                    Time = -1,
                    CompletedSteps = Enumerable.Empty<char>(),
                    TimeDelay = timeDelay
                };

        public static State CalculateTimeWithWorkers(string input, int noWorkers, int timeDelay) =>
            CreateInitialState(input, noWorkers, timeDelay)
                .WhileDo(x => x.StepsToDo.Any(), UpdateStateByOneSecond);
        

        private static bool StepCanBeStarted(this StepWithTime @this, IEnumerable<char> stepsCompleted) =>
            !@this.DependsOn.Any() || @this.DependsOn.All(stepsCompleted.Contains);

        private static InstructionLine ParseInstructionLine(this string @this) =>
            @this.SplitOnSpace().ToArray()
                .Map(x => new InstructionLine
                {
                    Step = x[7].Single(),
                    DependsOn = x[1].Single()
                });


        private static IEnumerable<char> GetStepsWithNoDependencies(IEnumerable<char> allSteps, IEnumerable<StepWithDependencies> parsedSteps) =>
            allSteps.Except(parsedSteps.Select(x => x.Step));

        private static IEnumerable<StepWithDependencies> ParseInstructionLines(this string @this) =>
            @this.SplitOnNewline()
                .Select(ParseInstructionLine).ToArray()
                .Fork(f1: x => x.Select(y => y.DependsOn).Distinct(),
                    f2: x => x.Map(i =>
                        i.GroupBy(y => y.Step)
                            .Select(y => new StepWithDependencies
                            {
                                Step = y.Key,
                                DependsOn = y.Select(z => z.DependsOn)
                            })
                    ).ToArray(),
                    join: (a, b) => 
                        GetStepsWithNoDependencies(a, b).Select(c => new StepWithDependencies
                        {
                            Step = c,
                            DependsOn = Enumerable.Empty<char>()
                        }).Concat(b));


        private class InstructionLine
        {
            public char Step { get; set; }
            public char DependsOn { get; set; }
        }

        private class StepWithDependencies
        {
            public char Step { get; set; }
            public IEnumerable<char> DependsOn { get; set; }
        }
    }


    public class Worker
    {
        public int Id { get; set; }
        public char CurrentStep { get; set; }
        public int SecondStepCompletedBy { get; set; }
    }

    public class StepWithTime
    {
        public char Step { get; set; }
        public int Time { get; set; }
        public bool Done { get; set; }
        public IEnumerable<char> DependsOn { get; set; }
    }

    public class State
    {
        public IEnumerable<Worker> Workers { get; set; }
        public int TimeDelay { get; set; }
        public IEnumerable<StepWithTime> StepsToDo { get; set; }
        public IEnumerable<char> CompletedSteps { get; set; }
        public int Time { get; set; }
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
            var output = InstructionCalculator.CalculateTimeWithWorkers(string.Join("\r\n", input), 2, 0);
            output.Time.Should().Be(15);
        }

        [Fact]
        public void Day07b()
        {
            var input = File.ReadAllText(".\\Content\\Day07.txt");
            var output = InstructionCalculator.CalculateTimeWithWorkers(string.Join("\r\n", input), 5, 60);
            output.Should().Be(948);
        }
    }
}
