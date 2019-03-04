using System.Linq;
using FluentAssertions;
using Xunit;

namespace Functional_CSharp
{
    public class Functional_Composition_01
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
            var bestStories = dwStories.Split("\r\n")
                                        .Select(x => x.Split(","))
                                        .Where(x => int.Parse(x[2]) >= 4)
                                        .Select(x => x[1])
                                        .OrderBy(x => x);
            bestStories.Should().BeEquivalentTo("Marco Polo", 
                                                "The Aztecs", 
                                                "The Dead Planet", 
                                                "The Edge of Destruction");
        }
    }
}
