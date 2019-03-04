using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Functional_CSharp
{
    public class Functional_Composition_02
    {
        private string dwStories = @"A,An Unearthly Child,3
                                    B,The Dead Planet,5,
                                    C,The Edge of Destruction,4
                                    D,Marco Polo,5
                                    E,The Keys of Marinus,2
                                    F,The Aztecs,4
                                    G,The Sensorites,1
                                    H,The Reign of Terror,3";

        private readonly Func<string, string[]> _splitOnComma = x => x.Split(",");
        private readonly Func<string[], EpisodeData> _parseData = x => new EpisodeData
        {
            SerialCode = x[0],
            Title = x[1],
            Rating = int.Parse(x[2])
        };
        private readonly Func<EpisodeData, bool> _getBestStories = x => x.Rating >= 4;

        [Fact]
        public void Test1()
        {
            var bestStories = dwStories.Split("\r\n")
                .Select(_splitOnComma)
                .Select(_parseData)
                .Where(_getBestStories)
                .Select(x => x.Title)
                .OrderBy(x => x);
            bestStories.Should().BeEquivalentTo("Marco Polo", 
                                                "The Aztecs", 
                                                "The Dead Planet", 
                                                "The Edge of Destruction");
        }
    }

    public class EpisodeData
    {
        public string SerialCode { get; set; }
        public string Title { get; set; }
        public int Rating { get; set; }
    }
}
