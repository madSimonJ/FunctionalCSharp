using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Functional_CSharp._01_Functional_Composition
{


    public class Functors_01
    {
        private readonly Func<string, string> DataBaseOperationThatReturns = x => $"this works: {x}";
        private readonly Func<string, string> DataBaseOperationThatDoesNotReturn = x => null;

        [Fact]
        public void TestAlt()
        {
            var input = "Simon Painter";
            var output = input.Alt(DataBaseOperationThatDoesNotReturn, DataBaseOperationThatReturns);
            output.Should().Be("this works: Simon Painter");
        }

    }

    public static class FunctionalExtensions
    {
        public static TOutput Alt<TInput, TOutput>(this TInput @this, Func<TInput, TOutput> f1,Func<TInput, TOutput> f2) =>
            f1(@this).IfDefaultDo(f2, @this);

        private static TOutput IfDefaultDo<TInput, TOutput>(this TOutput @this, Func<TInput, TOutput> elseF, TInput input) =>
            EqualityComparer<TOutput>.Default.Equals(@this, default(TOutput))
                ? elseF(input)
                : @this;

    }
}
