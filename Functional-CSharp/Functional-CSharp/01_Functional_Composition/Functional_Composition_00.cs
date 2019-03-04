using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Functional_CSharp
{
    public class Functional_Composition_00
    {
        private string dwStories = @"A,An Unearthly Child,3
                                    B,The Dead Planet,5,
                                    C,The Edge of Destruction,4
                                    D,Marco Polo,5
                                    E,The Keys of Marinus,2
                                    F,The Aztecs,4
                                    G,The Sensorites,1
                                    H,The Reign of Terror,3";

        [Fact]
        public void Test1()
        {
            var storiesSplitByRow = dwStories.Split("\r\n");
            var bestStories = new List<string>();
            foreach (var sr in storiesSplitByRow)
            {
                var rowData = sr.Split(",");
                var rating = int.Parse(rowData[2]);
                if (rating >= 4)
                {
                    bestStories.Add(rowData[1]);
                }
            }


            bestStories.Should().BeEquivalentTo("Marco Polo",
                "The Aztecs",
                "The Dead Planet",
                "The Edge of Destruction");
        }
    }
}
