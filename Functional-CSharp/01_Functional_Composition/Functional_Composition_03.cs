using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace Functional_CSharp
{
    public class Functional_Composition_03
    {
        public bool ValidateNino(string nino)
        {
            nino = nino.Replace(" ", "");

            if(nino.Length != 9)
            {
                return false;
            }

            var startsWithAlpha = new Regex("[a-zA-Z]").IsMatch(nino.Substring(0, 2));

            if (!startsWithAlpha)
            {
                return false;
            }

            var next6CharsAreDigits = new Regex("[0-9]").IsMatch(nino.Substring(2, 6));

            if (!next6CharsAreDigits)
            {
                return false;
            }

            var lastCharIsValid = new Regex("[A-D]").IsMatch(nino.Substring(8,1));

            if (!lastCharIsValid)
            {
                return false;
            }

            var firstCharIsInvalid = new Regex("[DFIQUV]").IsMatch(nino.Substring(0, 1));

            if (firstCharIsInvalid)
            {
                return false;
            }

            var secondCharIsInvalid = new Regex("[DFIOQUV]").IsMatch(nino.Substring(1, 1));

            if (secondCharIsInvalid)
            {
                return false;
            }

            return true;
        }

        [Fact]
        public void Test01()
        {
            var nino = "PX 02 46 17 D";
            ValidateNino(nino).Should().BeTrue();
        }
    }
}
