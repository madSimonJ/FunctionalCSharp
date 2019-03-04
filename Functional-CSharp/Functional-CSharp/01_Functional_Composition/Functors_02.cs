using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Functional_CSharp._01_Functional_Composition
{
    public class Functors_02
    {
        [Fact]
        public void TestFork()
        {
            var input = "aaabbbcccddd";
            var output = input.Fork(x => x.Sum(),
                x => x.Count(y => y == 'a'),
                x => x.Count(y => y == 'b'),
                x => x.Count(y => y == 'c'));
            output.Should().Be(9);
        }
    }

    public static class Functional_Extensions
    {
        public static TToType Map<TFromType, TToType>(this TFromType @this, Func<TFromType, TToType> f) =>
            f(@this);

        public static TOutput Fork<TInput, TOutput>(this TInput @this, Func<IEnumerable<TOutput>, TOutput> joinFunc, params Func<TInput, TOutput>[] prongs) =>
            prongs.Select(x => x(@this)).Map(joinFunc);
    }
}
