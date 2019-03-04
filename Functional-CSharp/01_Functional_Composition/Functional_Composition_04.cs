using System;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace Functional_CSharp
{
    public class FunctionalComposition04
    {
        private readonly Func<string, bool> _mustHaveLengthOf9 = x => x.Length == 9;
        private readonly Func<string, bool> _mustStartWithAlpha = x => new Regex("[a-zA-Z]").IsMatch(x.Substring(0, 2));
        private readonly Func<string, bool> _mustHaveNext6CharsAsDigits = x => new Regex("[0-9]").IsMatch(x.Substring(2, 6));
        private readonly Func<string, bool> _mustHaveAValidLastChar = x => new Regex("[A-D]").IsMatch(x.Substring(8, 1));
        private readonly Func<string, bool> _mustHaveAValidFirstChar = x => new Regex("[^DFIQUV]").IsMatch(x.Substring(0, 1));
        private readonly Func<string, bool> _mustHaveAValidSecondChar = x => new Regex("[^DFIOQUV]").IsMatch(x.Substring(1, 1));

        public bool ValidateNino(string nino) =>
            new[]
            {
                _mustHaveLengthOf9,
                _mustStartWithAlpha,
                _mustHaveNext6CharsAsDigits,
                _mustHaveAValidLastChar,
                _mustHaveAValidFirstChar,
                _mustHaveAValidSecondChar
            }.All(x => x(nino.Replace(" ", "")));

        [Fact]
        public void Test01()
        {
            var nino = "PX 02 46 17 D";
            ValidateNino(nino).Should().BeTrue();
        }
    }

    public class FunctionalComposition04b
    {
        private readonly Func<string, bool> _mustHaveLengthOf9 = x => x.Length == 9;
        private readonly Func<string, bool> _mustStartWithAlpha = x => new Regex("[a-zA-Z]").IsMatch(x.Substring(0, 2));
        private readonly Func<string, bool> _mustHaveNext6CharsAsDigits = x => new Regex("[0-9]").IsMatch(x.Substring(2, 6));
        private readonly Func<string, bool> _mustHaveAValidLastChar = x => new Regex("[A-D]").IsMatch(x.Substring(8, 1));
        private readonly Func<string, bool> _mustHaveAValidFirstChar = x => new Regex("[^DFIQUV]").IsMatch(x.Substring(0, 1));
        private readonly Func<string, bool> _mustHaveAValidSecondChar = x => new Regex("[^DFIOQUV]").IsMatch(x.Substring(1, 1));

        public bool ValidateNino(string nino) =>
            nino.Replace(" ", "")
                .Validate(
                    _mustHaveLengthOf9,
                    _mustStartWithAlpha,
                    _mustHaveNext6CharsAsDigits,
                    _mustHaveAValidLastChar,
                    _mustHaveAValidFirstChar,
                    _mustHaveAValidSecondChar
                );

        [Fact]
        public void Test01()
        {
            var nino = "PX 02 46 17 D";
            ValidateNino(nino).Should().BeTrue();
        }
    }

    public static class FunctionalExtensions
    {
        public static bool Validate<TInput>(this TInput @this, params Func<TInput, bool>[] predicates) =>
            predicates.All(x => x(@this));
    }


    public class FunctionalComposition04c
    {
        public bool ValidateNino(string nino) =>
            nino.Replace(" ", "")
                .Validate(
                    x => x.HasLengthOf(9),
                    x => x.StartChars(2).IsAlphaNumeric(),
                    x => x.CharsBetween(2, 6).AreAllNumeric(),
                    x => x.Last().IsOneOfCharSet("A-D"),
                    x => x.First().IsNotOneOfCharSet("DFIQUV"),
                    x => x.Second().IsNotOneOfCharSet("DFIOQUV")
                );

        [Fact]
        public void Test01()
        {
            var nino = "PX 02 46 17 D";
            ValidateNino(nino).Should().BeTrue();
        }


    }

    public static class StringExtensions
    {
        public static bool IsRegexMatch(this string @this, string pattern) => new Regex(pattern).IsMatch(@this);

        public static bool HasLengthOf(this string @this, int l) => @this.Length == l;
        public static bool IsAlphaNumeric(this string @this) => @this.IsRegexMatch("[a-zA-Z]");
        public static bool AreAllNumeric(this string @this) => @this.IsRegexMatch("[0-9]");
        public static bool IsOneOfCharSet(this char @this, string charSet) => @this.ToString().IsRegexMatch($"[{charSet}]");

        public static bool IsNotOneOfCharSet(this char @this, string charSet) => IsOneOfCharSet(@this, charSet).Not();

        public static string CharsBetween(this string @this, int from, int to) =>
            new string(@this.Skip(from).Take(to).ToArray());

        public static string StartChars(this string @this, int noChars) => new string(@this.Take(noChars).ToArray());
        public static bool Not(this bool @this) => !@this;

        public static char Second(this string @this) => @this[1];
    }


}
