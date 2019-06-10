using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace AdventOfCode
{
    public static class GuardCalculator
    {
        private static DateTime GetDateTime(this string input) =>
            Regex.Match(input, "\\[(.*?)\\]")
                .Value.Replace("[", "").Replace("]", "")
                .Map(DateTime.Parse);

        private static string GetMessage(this string input) =>
            input.Substring(input.IndexOf(']') + 1).TrimStart();

        //public static (DateTime date, string message) Parse(this string input) =>
        //    (input.GetDateTime(), input.GetMessage());

        public static IEnumerable<(DateTime date, string message)> Parse(this string input) =>
            input.SplitOnNewline()
                .Select(x => (Date: x.GetDateTime(), Message: x.GetMessage()))
                .OrderBy(x => x.Date);

        private static int GetGuard(string message, int currentGuard) =>
            message.StartsWith("Guard #")
                ? message.Split(" ")[1].Replace("#", "").Map(int.Parse)
                : currentGuard;

        private static IDictionary<int, IEnumerable<(DateTime Date, string Message)>> UpdateExisting(this IDictionary<int, IEnumerable<(DateTime Date, string Message)>> @this, (DateTime Date, string Message) newMessage, int guardId) =>
            @this.Where(x => x.Key != guardId)
                .Concat(new[] { new KeyValuePair<int, IEnumerable<(DateTime Date, string Message)>>(guardId, @this[guardId].Concat(new[] {newMessage}))})
                .ToDictionary(x => x.Key, x => x.Value);

        private static IDictionary<int, IEnumerable<(DateTime Date, string Message)>> AddNew(this IDictionary<int, IEnumerable<(DateTime Date, string Message)>> @this, (DateTime Date, string Message) newMessage, int guardId) =>
            @this.Concat(new[] { new KeyValuePair<int, IEnumerable<(DateTime Date, string Message)>>(guardId, new[] { newMessage }) })
                .ToDictionary(x => x.Key, x => x.Value);

        private static IDictionary<int, IEnumerable<(DateTime Date, string Message)>> UpdateDictionary(this IDictionary<int, IEnumerable<(DateTime Date, string Message)>> @this, (DateTime Date, string Message) newMessage, int guardId) =>
            @this.ContainsKey(guardId)
                ? @this.UpdateExisting(newMessage, guardId)
                : @this.AddNew(newMessage, guardId);
                //? @this.Where(x => x.Key != guardId).Concat( @this[guardId].Concat(new [] {newMessage}) )

        private static IDictionary<int, IEnumerable<(DateTime Date, string Message)>> GroupMessagesByGuard(IEnumerator<(DateTime date, string message)> e, IDictionary<int, IEnumerable<(DateTime Date, string Message)>> progress, int currentGuard = -1) =>
            e.MoveNext()
                ? GetGuard(e.Current.message, currentGuard)
                    .Map(x => GroupMessagesByGuard(e, progress.UpdateDictionary(e.Current, x), x))
                : progress;

        public static IDictionary<int, IEnumerable<(DateTime Date, string Message)>> GroupMessagesByGuard(IEnumerable<(DateTime date, string message)> input) =>
            GroupMessagesByGuard(input.GetEnumerator(), new Dictionary<int, IEnumerable<(DateTime Date, string Message)>>());



        private static (int key, IEnumerable<DateTime> Time) GetMessagesOfType(this KeyValuePair<int, IEnumerable<(DateTime Date, string Message)>> input, string message) =>
            (input.Key, input.Value.Where(y => y.Message == message).Select(z => z.Date));


        private static (T From, T To) GetPair<T>(this IEnumerator<T> @this) =>
            @this.Current.Map(x => @this.MoveNext().Map(y => (x, @this.Current)));

        private static IEnumerable<TimeSpan> GroupMessagesToTimespans(IEnumerator<(DateTime Date, string Message)> e, IEnumerable<TimeSpan> progress) =>
            e.MoveNext()
                ? e.Current.Message == "falls asleep"
                    ?  GroupMessagesToTimespans(e, progress.Concat(new[] {e.GetPair().Map(x => x.To.Date - x.From.Date)}))
                    : GroupMessagesToTimespans(e, progress)
                : progress;

        private static IEnumerable<(int Minute, int NumberOfSleeps)> UpdateSleeps(IEnumerable<(int Minute, int NumberOfSleeps)> progress, ((DateTime Date, string Message) dt1, (DateTime Date, string Message) dt2) update) =>
            progress.Select(x => (x.Minute,
                    x.Minute >= update.dt1.Date.Minute && x.Minute < update.dt2.Date.Minute
                        ? x.NumberOfSleeps + 1
                        : x.NumberOfSleeps
                )
            );

        private static IEnumerable<(int Minute, int NumberOfSleeps)> GetSleepyMinutes(IEnumerator<(DateTime date, string Messae)> e, IEnumerable<(int Minute, int NumberOfSleeps)> progress) =>
            e.MoveNext()
                ? e.Current.Messae == "falls asleep"
                      ? GetSleepyMinutes(e, UpdateSleeps(progress, e.GetPair()))
                      : GetSleepyMinutes(e, progress)
                : progress;

        private static IDictionary<int, IEnumerable<TimeSpan>> GroupMessagesToTimespans(IDictionary<int, IEnumerable<(DateTime Date, string Message)>> input) =>
            input.Select(x => (Key: x.Key, Value: GroupMessagesToTimespans(x.Value.GetEnumerator(), Enumerable.Empty<TimeSpan>())))
                .ToDictionary(x => x.Key,
                    x => x.Value);

        private static IEnumerable<(int Minute, int NumberOfSleeps)> GetStartingMinutes() =>
            Enumerable.Range(0, 59)
                .Select(x => (x, 0));

        public static int GetSleepiestGuard(string input) =>
            input.Parse()
                .Map(GroupMessagesByGuard)
                .Map(x => ( 
                    Messages: x, 
                    SleepDetails:  x.Map(GroupMessagesToTimespans)
                                    .Map(a => a.Select(b => (GuardId: b.Key, SleepPeriods: b.Value.Sum(c => c.Minutes))))
                            ) 
                )
                .Map(x => (
                        Messages: x.Messages, 
                        SleepiestGuardDetails: x.SleepDetails.Single(y => y.SleepPeriods == x.SleepDetails.Max(z => z.SleepPeriods))  
                    ))
                .Map(x => (
                        Messages: x.Messages[x.SleepiestGuardDetails.GuardId],
                        SleepiestGuard: x.SleepiestGuardDetails.GuardId
                    ))
                .Map(x => (
                    SleepiestGuard: x.SleepiestGuard,
                    SleepyMinutes: GetSleepyMinutes(x.Messages.GetEnumerator(), GetStartingMinutes())
                ))
                .Map(x => (
                    SleepiestGuard: x.SleepiestGuard,
                    SleepiestMinute: x.SleepyMinutes.Single(y => y.NumberOfSleeps == x.SleepyMinutes.Max(z => z.NumberOfSleeps)).Minute
                ))
                .Map(x => x.SleepiestGuard * x.SleepiestMinute);

        public static object GetSleepiestGuard2(string input) =>
            input.Parse()
                .Map(GroupMessagesByGuard)
                .Map(x => x.Select(y => (
                    GuardId: y.Key,
                    SleepyMinutes: GetSleepyMinutes(y.Value.GetEnumerator(), GetStartingMinutes())
                )))
                .Map(x => (
                    SleepyDetails: x,
                    SleepiestMinute: x.SelectMany(y => y.SleepyMinutes).Max(z => z.NumberOfSleeps)
                ))
                .Map(x => (
                    SleepiestGuard: x.SleepyDetails.Single(y => y.SleepyMinutes.Any(z => z.NumberOfSleeps == x.SleepiestMinute))  ,
                    SleepiestMinute: x.SleepiestMinute
                ))
                .Map(x => (
                    SleepiestMinute: x.SleepiestGuard.SleepyMinutes.Single(y => y.NumberOfSleeps == x.SleepiestMinute).Minute,
                    SleepiestGuard: x.SleepiestGuard.GuardId
                ))
                .Map(x => x.SleepiestGuard * x.SleepiestMinute);

    }

    public class Day04
    {
        [Fact]
        public void Day04a_Test()
        {
            var input = "[1518-11-01 00:00] Guard #10 begins shift\r\n[1518-11-01 00:05] falls asleep\r\n[1518-11-01 00:25] wakes up\r\n[1518-11-01 00:30] falls asleep\r\n[1518-11-01 00:55] wakes up\r\n[1518-11-01 23:58] Guard #99 begins shift\r\n[1518-11-02 00:40] falls asleep\r\n[1518-11-02 00:50] wakes up\r\n[1518-11-03 00:05] Guard #10 begins shift\r\n[1518-11-03 00:24] falls asleep\r\n[1518-11-03 00:29] wakes up\r\n[1518-11-04 00:02] Guard #99 begins shift\r\n[1518-11-04 00:36] falls asleep\r\n[1518-11-04 00:46] wakes up\r\n[1518-11-05 00:03] Guard #99 begins shift\r\n[1518-11-05 00:45] falls asleep\r\n[1518-11-05 00:55] wakes up";
            var output = GuardCalculator.GetSleepiestGuard(input);
            output.Should().Be(240);
        }

        [Fact]
        public void Day04a()
        {
            var input = File.ReadAllText(".\\Content\\Day04.txt");
            var output = GuardCalculator.GetSleepiestGuard(input);
            output.Should().Be(73646);
        }

        [Fact]
        public void Day04b_Test()
        {
            var input = "[1518-11-01 00:00] Guard #10 begins shift\r\n[1518-11-01 00:05] falls asleep\r\n[1518-11-01 00:25] wakes up\r\n[1518-11-01 00:30] falls asleep\r\n[1518-11-01 00:55] wakes up\r\n[1518-11-01 23:58] Guard #99 begins shift\r\n[1518-11-02 00:40] falls asleep\r\n[1518-11-02 00:50] wakes up\r\n[1518-11-03 00:05] Guard #10 begins shift\r\n[1518-11-03 00:24] falls asleep\r\n[1518-11-03 00:29] wakes up\r\n[1518-11-04 00:02] Guard #99 begins shift\r\n[1518-11-04 00:36] falls asleep\r\n[1518-11-04 00:46] wakes up\r\n[1518-11-05 00:03] Guard #99 begins shift\r\n[1518-11-05 00:45] falls asleep\r\n[1518-11-05 00:55] wakes up";
            var output = GuardCalculator.GetSleepiestGuard2(input);
            output.Should().Be(4455);
        }

        [Fact]
        public void Day04b()
        {
            var input = File.ReadAllText(".\\Content\\Day04.txt");
            var output = GuardCalculator.GetSleepiestGuard2(input);
            output.Should().Be(4727);
        }
    }
}
