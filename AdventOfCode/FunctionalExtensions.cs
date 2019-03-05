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

        public static TOutput Fork<TInput, TOutput>(this TInput @this, Func<IEnumerable<TOutput>, TOutput> joinFunc, params Func<TInput, TOutput>[] prongs) =>
            prongs.Select(x => x(@this)).Map(joinFunc);

        public static TOutput Fork<TInput, TIntermediate, TOutput>(this TInput @this, 
                                                                        Func<IEnumerable<TIntermediate>, TOutput> joinFunc, 
                                                                        params Func<TInput, TIntermediate>[] prongs) =>
            prongs.Select(x => x(@this)).Map(joinFunc);

    }
}