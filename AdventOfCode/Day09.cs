using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace AdventOfCode
{
    public static class ScoreCalculator
    {
        private static int PositionOfLastMarble(this int[] board) =>
            board.Select((x, i) => (Number: x, Index: i)).Max().Index;



        private static int PositionOfNewMarble(int lastMarblePos, int lengthOfNewArray) =>
           (lastMarblePos + 2)
                .Match(
                    (x => x == lengthOfNewArray, _ => 1),
                    (_ => true, x => x)
               );

        public static (IEnumerable<int> Board, int Score) UpdateBoard(int[] board) =>
            board.Length.Match(
                    (x => x.IsDivisibleBy(23), _ => (Enumerable.Empty<int>(), 0)),
                    (_ => true, x => (board.InsertAtPosition(PositionOfNewMarble(board.PositionOfLastMarble(), x + 1), x), 0))
                );


        public static IEnumerable<int> CalculateMarbles(int highestValueOfMarble, int[] board = null) =>
            (board ?? new[] { 0 })
                .Map(x =>
                    x.Length == (highestValueOfMarble + 1)
                        ? x
                        : CalculateMarbles(highestValueOfMarble, UpdateBoard(x).Board.ToArray())
                );
    }


    public class Day09
    {
        [Fact]
        public void Day09_a_Test_01()
        {
            var result = ScoreCalculator.CalculateMarbles(0);
            result.Should().Equal(0);
        }

        [Fact]
        public void Day09_a_Test_02()
        {
            var result = ScoreCalculator.CalculateMarbles(1);
            result.Should().Equal(0, 1);
        }

        [Fact]
        public void Day09_a_Test_03()
        {
            var result = ScoreCalculator.CalculateMarbles(2);
            result.Should().Equal(0, 2, 1);
        }

        [Fact]
        public void Day09_a_Test_04()
        {
            var result = ScoreCalculator.CalculateMarbles(3);
            result.Should().Equal(0, 2, 1, 3);
        }

        [Fact]
        public void Day09_a_Test_05()
        {
            var result = ScoreCalculator.CalculateMarbles(4);
            result.Should().Equal(0, 4, 2, 1, 3);
        }

        [Fact]
        public void Day09_a_Test_06()
        {
            var result = ScoreCalculator.CalculateMarbles(5);
            result.Should().Equal(0, 4, 2, 5, 1, 3);
        }

        [Fact]
        public void Day09_a_Test_07()
        {
            var result = ScoreCalculator.CalculateMarbles(22);
            result.Should().Equal(0, 16,  8, 17,  4, 18,  9, 19,  2, 20, 10, 21,  5, 22, 11,  1, 12,  6, 13,  3, 14,  7, 15);
        }

        [Fact]
        public void Day09_a_Test_08()
        {
            var result = ScoreCalculator.CalculateMarbles(23);
            result.Should().Equal(0, 16,  8, 17,  4, 18, 19, 2, 20, 10, 21,  5, 22, 11,  1, 12,  6, 13,  3, 14,  7, 15);
        }
        

    }
}
