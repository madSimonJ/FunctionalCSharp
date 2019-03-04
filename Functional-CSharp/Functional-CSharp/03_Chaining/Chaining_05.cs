using System;
using System.Collections.Generic;
using FluentAssertions;
using Functional_CSharp._04_Chaining;
using Xunit;

namespace Functional_CSharp._05_Chaining
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
            string formattedPerson = personId.ToMaybe()
                .Bind(GetPerson)
                .Bind(x => $"{x.FirstName} {x.LastName} ({x.Age})")
                .Bind(x => x.Replace("a", "4"))
                .Bind(x => x.Replace("e", "3"))
                .Bind(x => x.Replace("i", "1"))
                .Bind(x => x.Replace("o", "0"));

            formattedPerson.Should().Be("S1m0n P41nt3r (36)");
        }


        public Func<int, Person> GetPerson2 = (int personId) =>
            throw new Exception("Arrgh!");

        [Fact]
        public void Test02()
        {
            var personId = 12;
            var formattedPerson = personId.ToEither()
                .Bind(GetPerson2)
                .Bind(x => $"{x.FirstName} {x.LastName} ({x.Age})")
                .Bind(x => x.Replace("a", "4"))
                .Bind(x => x.Replace("e", "3"))
                .Bind(x => x.Replace("i", "1"))
                .Bind(x => x.Replace("o", "0"));

            formattedPerson.Should().BeOfType<Left<string>>();
            var left = formattedPerson as Left<string>;
            left.Exception.Message.Should().Be("Arrgh!");
        }

    }

    public abstract class Either<T>
    {
        public abstract T Value { get; }
        public static implicit operator T(Either<T> @this) => @this.Value;
    }

    public class Right<T> : Either<T>
    {
        public override T Value { get; }

        public Right(T val)
        {
            Value = val;
        }
    }

    public class Left<T> : Either<T>
    {
        public override T Value => default(T);
        public Exception Exception { get; set; }

        public Left(Exception e)
        {
            Exception = e;
        }
    }

    public static class FunctionalExtensions3
    {
        public static Either<T> ToEither<T>(this T value) => new Right<T>(value);

        public static Either<TToType> Bind<TFromType, TToType>(this Either<TFromType> @this, Func<TFromType, TToType> f)
        {
            switch (@this)
            {
                case Right<TFromType> rgt when !EqualityComparer<TFromType>.Default.Equals(rgt.Value, default(TFromType)):
                    try
                    {
                        return f(rgt.Value).ToEither();
                    }
                    catch (Exception e)
                    {
                        return new Left<TToType>(e);
                    }
                case Left<TFromType> lft:
                    return new Left<TToType>(lft.Exception);
                default:
                    return new Left<TToType>(new Exception("Default value"));
            }
        }
    }
}
