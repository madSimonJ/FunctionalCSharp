using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Functional_CSharp
{
    public class Functional_Composition_06
    {
        [Fact]
        public void LowIncome_PaysNoTax() =>
            ApplyTaxToSalary(11000).Should().Be(11000);

        [Fact]
        public void Band1_PaysPoint2Interest() =>
            ApplyTaxToSalary(46000).Should().Be(36800M);

        [Fact]
        public void Band2_PaysPoint4Interest() =>
            ApplyTaxToSalary(140000).Should().Be(84000M);

        [Fact]
        public void HighIncome_PaysPoint45Interest() =>
            ApplyTaxToSalary(160000).Should().Be(88000M);



        public decimal ApplyTaxToSalary(decimal salary) =>
            new (Func<decimal, bool> condition, Func<decimal, decimal> calculator)[]
            {
                (x => x <= 11850, x => x),
                (x => x <= 46350, x => x * 0.8M),
                (x => x <= 150000, x => x * 0.6M),
                (x => x > 150000, x => x * 0.55M),

            }.First(x => x.condition(salary)).calculator(salary);
    }

}
