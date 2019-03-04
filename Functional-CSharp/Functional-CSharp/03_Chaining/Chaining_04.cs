using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Functional_CSharp._04_Chaining
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }

    public class Chaining_04
    {
        public Func<int, Person> GetPerson = (int personId) =>
            new Person
            {

                FirstName = "Simon",
                LastName = "Painter",
                Age = 36
            };

        [Fact]
        public void Test01()
        {
            var personId = 12;
            string formattedPerson = personId.ToIdentity()
                .Bind(GetPerson)
                .Bind(x => $"{x.FirstName} {x.LastName} ({x.Age})")
                .Bind(x => x.Replace("a", "4"))
                .Bind(x => x.Replace("e", "3"))
                .Bind(x => x.Replace("i", "1"))
                .Bind(x => x.Replace("o", "0"));

            formattedPerson.Should().Be("S1m0n P41nt3r (36)");
        }


        public Func<int, Person> GetPerson2 = (int personId) =>
            null;

        [Fact]
        public void Test02()
        {
            var personId = 12;
            string formattedPerson = personId.ToMaybe()
                .Bind(GetPerson2)
                .Bind(x => $"{x.FirstName} {x.LastName} ({x.Age})")
                .Bind(x => x.Replace("a", "4"))
                .Bind(x => x.Replace("e", "3"))
                .Bind(x => x.Replace("i", "1"))
                .Bind(x => x.Replace("o", "0"));

            formattedPerson.Should().BeNull();
        }

    }


    public class Identity<T>
    {
        public T Value { get; }

        public Identity(T value)
        {
            Value = value;
        }

        public static implicit operator Identity<T>(T @this) => @this.ToIdentity();
        public static implicit operator T(Identity<T> @this) => @this.Value;
    }

    public static class FunctionalExtensions2
    {
        public static Identity<T> ToIdentity<T>(this T @this) => new Identity<T>(@this);

        public static Identity<TToType> Bind<TFromType, TToType>(this Identity<TFromType> @this,
            Func<TFromType, TToType> f) =>
            f(@this.Value).ToIdentity();
    }

    public abstract class Maybe<T>
    {
        public abstract T Value { get; }
        public static implicit operator T(Maybe<T> @this) => @this.Value;
    }

    public class Just<T> : Maybe<T>
    {
        public override T Value { get; }

        public Just(T val)
        {
            Value = val;
        }
    }

    public class Nothing<T> : Maybe<T>
    {
        public override T Value => default(T);
    }

    public static class FunctionalExtensions3
    {
        public static Maybe<T> ToMaybe<T>(this T value) => new Just<T>(value);

        public static Maybe<TToType> Bind<TFromType, TToType>(this Maybe<TFromType> @this, Func<TFromType, TToType> f)
        {
            switch (@this)
            {
                case Just<TFromType> sth when !EqualityComparer<TFromType>.Default.Equals(sth.Value, default(TFromType)):
                    try
                    {
                        return f(sth).ToMaybe();
                    }
                    catch (Exception)
                    {
                        return new Nothing<TToType>();
                    }
                default:
                    return new Nothing<TToType>();
            }
        }
    }
}
