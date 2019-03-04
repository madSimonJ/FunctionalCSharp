using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Functional_CSharp._02_Currying_and_Partial_Application
{
    public class Partial_Application
    {
        private string dwStories = @"A,An Unearthly Child,3
                                    B,The Dead Planet,5,
                                    C,The Edge of Destruction,4
                                    D,Marco Polo,5
                                    E,The Keys of Marinus,2
                                    F,The Aztecs,4
                                    G,The Sensorites,1
                                    H,The Reign of Terror,3";

        private static readonly Func<string, char, int, string, IEnumerable<string>> _parseExtractAndConvert =
            (a, b, c, d) => d.Split(a)
                .Select(x => x.Split(b))
                .Select(x => x[c].TrimStart().TrimEnd());



        [Fact]
        public void Test1()
        {
            _parseExtractAndConvert("\r\n", ',', 1, dwStories).Should().BeEquivalentTo(
                "An Unearthly Child",
                "The Dead Planet",
                "The Edge of Destruction",
                "Marco Polo",
                "The Keys of Marinus",
                "The Aztecs",
                "The Sensorites",
                "The Reign of Terror"
            );
        }


        private readonly Func<int, string, IEnumerable<string>> _parse = _parseExtractAndConvert.Apply("\r\n", ',');

        [Fact]
        public void Test2()
        {
            var getCodes = _parse.Apply(0);
            getCodes(dwStories).Should().BeEquivalentTo("A", "B", "C", "D", "E", "F", "G", "H");

        }

        [Fact]
        public void Test3()
        {
            var getTitles = _parse.Apply(1);
            getTitles(dwStories).Should().BeEquivalentTo(
                "An Unearthly Child",
                "The Dead Planet",
                "The Edge of Destruction",
                "Marco Polo",
                "The Keys of Marinus",
                "The Aztecs",
                "The Sensorites",
                "The Reign of Terror"
            );
        }
    }

    public static class ApplyExtensions
    {
        public static Func<T3, T4, TR> Apply<T1, T2, T3, T4, TR>(this Func<T1, T2, T3, T4, TR> @this, T1 t1, T2 t2) =>
            (t3, t4) => @this(t1, t2, t3, t4);

        public static Func<T2, TR> Apply<T1, T2, TR>(this Func<T1, T2, TR> @this, T1 t1) =>
            t2 => @this(t1, t2);
    }
}
