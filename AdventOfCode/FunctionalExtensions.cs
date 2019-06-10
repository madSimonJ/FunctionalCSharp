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
    }

    public static class F
    {
        public static TOutput Fork<TInput1, TInput2, TOutput>(Func<TInput1> f1, Func<TInput2> f2, Func<TInput1, TInput2, TOutput> f3) =>
            f3(f1(), f2());
    }
}