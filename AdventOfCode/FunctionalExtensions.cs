using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode
{
    public static class FunctionalExtensions
    {

        public static TToType Map<TFromType, TToType>(this TFromType @this, Func<TFromType, TToType> f) =>
            f(@this);
        public static IEnumerable<string> SplitOnNewline(this string @this) =>
            Regex.Split(@this, "\r\n");

        public static IEnumerable<string> SplitOnSpace(this string @this) =>
            Regex.Split(@this, " ");

        public static TOutput Fork<TInput, TOutput>(this TInput @this, Func<IEnumerable<TOutput>, TOutput> joinFunc, params Func<TInput, TOutput>[] prongs) =>
            prongs.Select(x => x(@this)).Map(joinFunc);

        public static TOutput Fork<TInput, TIntermediate, TOutput>(this TInput @this, 
                                                                        Func<IEnumerable<TIntermediate>, TOutput> joinFunc, 
                                                                        params Func<TInput, TIntermediate>[] prongs) =>
            prongs.Select(x => x(@this)).Map(joinFunc);

        public static TOutput Fork<TInput, TIntermediate1, TIntermediate2, TOutput>(this TInput @this, Func<TInput, TIntermediate1> f1, Func<TInput, TIntermediate2> f2, Func<TIntermediate1, TIntermediate2, TOutput> join) =>
            join(f1(@this), f2(@this));

        public static T AggregateFunc<T>(IEnumerator<Func<T, T>> e, T agg = default(T)) =>
            e.MoveNext()
                ? AggregateFunc(e, e.Current(agg))
                : agg;

        public static T AggregateFunc<T>(this IEnumerable<Func<T, T>> array) =>
            AggregateFunc(array.GetEnumerator());

        public static T WhileDo<T>(this T @this, Func<T, bool> predicate, Func<T, T> updateF) =>
            predicate(@this)
                ? updateF(@this).WhileDo(predicate, updateF)
                : @this;

        public static T GetNext<T>(this IEnumerator<T> @this) =>
            @this.MoveNext()
                ? @this.Current
                : default(T);

        public static IEnumerable<T> Repeat<T>(this Func<T> @this, int times) =>
            Enumerable.Repeat(@this, times)
                .Select(x => x());

        public static IEnumerable<TOutput> Repeat<TInput, TOutput>(this TInput @this, Func<TInput, TOutput> f, int times) =>
            Enumerable.Repeat(@this, times)
            .Select(x => f(x));

        public static T IfThen<T>(this T @this, Func<T, bool> predicate, Func<T, T> f) =>
            predicate(@this)
                ? f(@this)
                : @this;

        public static IEnumerable<T> GetAtPositions<T>(this IEnumerable<T> @this, IEnumerable<int> positions) =>
            @this.Where((_, i) => positions.Contains(i));

        public static IEnumerable<T> AsArray<T>(this T @this) =>
            new[] { @this };

        public static IEnumerable<T> InsertAtPosition<T>(this IEnumerable<T> @this, int pos, T item) =>
            @this.Take(pos)
            .Append(item)
            .Concat(@this.Skip(pos));

        public static TOutput Match<TInput, TOutput>(this TInput @this, params (Func<TInput, bool> cond, Func<TInput, TOutput> trans)[] f) =>
            f.First(x => x.cond(@this)).trans(@this);

        public static bool IsDivisibleBy(this int @this, int divisor) =>
            @this % divisor == 0;

   }

    public static class F
    {
        public static TOutput Fork<TInput1, TInput2, TOutput>(Func<TInput1> f1, Func<TInput2> f2, Func<TInput1, TInput2, TOutput> f3) =>
            f3(f1(), f2());
    }
}