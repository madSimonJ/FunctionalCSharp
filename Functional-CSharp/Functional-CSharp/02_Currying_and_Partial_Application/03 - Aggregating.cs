using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using Xunit;

namespace Functional_CSharp._02_Currying_and_Partial_Application
{
    public class _03___Aggregating
    {
        private IEnumerable<string> dwStories = new[]
        {
            "An Unearthly Child",
            "The Dead Planet",
            "The Edge of Destruction",
            "Marco Polo",
            "The Keys of Marinus",
            "The Aztecs",
            "The Sensorites",
            "The Reign of Terror"
        };


        [Fact]
        public void the_strings_should_be_merged()
        {
            var dwStoriesArray = dwStories.ToArray();
            var sb = new StringBuilder();
            for (var i = 0; i < dwStoriesArray.Length; i++)
            {
                sb.Append(dwStoriesArray[i]);
                if (i < (dwStoriesArray.Length - 1))
                {
                    sb.AppendLine(",");
                }
            }

            var mergedString = sb.ToString();
            mergedString.Should().Be("An Unearthly Child,\r\nThe Dead Planet,\r\nThe Edge of Destruction,\r\nMarco Polo,\r\nThe Keys of Marinus,\r\nThe Aztecs,\r\nThe Sensorites,\r\nThe Reign of Terror");
        }

        [Fact]
        public void the_strings_should_be_merged_functionally()
        {
            var mergedString = string.Join(",\r\n", dwStories);
            mergedString.Should().Be("An Unearthly Child,\r\nThe Dead Planet,\r\nThe Edge of Destruction,\r\nMarco Polo,\r\nThe Keys of Marinus,\r\nThe Aztecs,\r\nThe Sensorites,\r\nThe Reign of Terror");
        }



        public class Story
        {
            public char Code { get; set; }
            public string Title { get; set; }
            public int Rating { get; set; }
            public int NumberOfEpisodes { get; set; }
            public int NumberOfMissingEpisodes { get; set; }
        }

        private IEnumerable<Story> StoryData = new[]
        {
            new Story
            {
                Code = 'A',
                Title =  "An Unearthly Child",
                Rating = 3,
                NumberOfEpisodes = 4,
                NumberOfMissingEpisodes = 0

            },
            new Story
            {
                Code = 'B',
                Title =  "The Dead Planet",
                Rating = 5,
                NumberOfEpisodes = 7,
                NumberOfMissingEpisodes = 0
            },
            new Story
            {
                Code = 'C',
                Title =  "The Edge of Destruction",
                Rating = 4,
                NumberOfEpisodes = 2,
                NumberOfMissingEpisodes = 0
            },
            new Story
            {
                Code = 'D',
                Title =   "Marco Polo",
                Rating = 5,
                NumberOfEpisodes = 7,
                NumberOfMissingEpisodes = 7
            },
            new Story
            {
                Code = 'E',
                Title =  "The Keys of Marinus",
                Rating = 2,
                NumberOfEpisodes = 6,
                NumberOfMissingEpisodes = 0
            },
            new Story
            {
                Code = 'F',
                Title =  "The Aztecs",
                Rating = 4,
                NumberOfEpisodes = 4,
                NumberOfMissingEpisodes = 0
            },
            new Story
            {
                Code = 'G',
                Title =  "The Sensorites",
                Rating = 1,
                NumberOfEpisodes = 6,
                NumberOfMissingEpisodes = 0
            },
            new Story
            {
                Code = 'H',
                Title =  "The Reign of Terror",
                Rating = 3,
                NumberOfEpisodes = 6,
                NumberOfMissingEpisodes = 2
            }
        };

        [Fact]
        public void there_should_be_a_total_number_of_episodes()
        {
            var totalEpisodes = StoryData.Sum(x => x.NumberOfEpisodes);
            totalEpisodes.Should().Be(42);
        }

        [Fact]
        public void there_should_be_a_total_number_of_existing_episodes()
        {
            var existingEpisodes = StoryData.Sum(x => x.NumberOfEpisodes - x.NumberOfMissingEpisodes);
            existingEpisodes.Should().Be(33);
        }

        [Fact]
        public void there_should_be_an_average_rating()
        {
            var average = StoryData.Average(x => x.Rating);
            average.Should().Be(3.375);
        }

        [Fact]
        public void there_should_be__percentage_of_missing_episodes()
        {
            var (totalMissing, totalOverall) = StoryData.Aggregate((0, 0), (acc, curr) => ( acc.Item1 + curr.NumberOfMissingEpisodes, acc.Item2 + curr.NumberOfEpisodes ) );
            var percentageMissing = Math.Round(totalMissing / (decimal)totalOverall * 100, 2);
            percentageMissing.Should().Be(21.43M);
        }
    }
}
