using System;
using FluentAssertions;
using Xunit;

namespace Functional_CSharp
{
    public class StandardBankAccount
    {
        public decimal Balance { get; set; }
        public decimal InterestRate { get; set; }
    }

    public class SuperDuperBankAccount : StandardBankAccount
    {
        public decimal BonusInterestRate { get; set; }
    }

    public class DodgyBankAccount : StandardBankAccount
    {
        public decimal BrownPaperBag { get; set; }
    }

    public class FunctionalComposition05
    {
        [Fact]
        public void StandardBankAccount_Under10000_GetsNormalInterest()
        {
            var bankAccount = new StandardBankAccount
            {
                Balance = 5000,
                InterestRate = 0.015M
            };
            var interest = CalculateInterest(bankAccount);
            interest.Should().Be(75M);
        }

        [Fact]
        public void StandardBankAccount_Over10000_GetsDoubleInterest()
        {
            var bankAccount = new StandardBankAccount
            {
                Balance = 15000,
                InterestRate = 0.015M
            };
            var interest = CalculateInterest(bankAccount);
            interest.Should().Be(450M);
        }

        [Fact]
        public void SuperDuperBankAccount_GetsBonusInterest()
        {
            var bankAccount = new SuperDuperBankAccount
            {
                Balance = 5000,
                InterestRate = 0.015M,
                BonusInterestRate = 0.02M
            };
            var interest = CalculateInterest(bankAccount);
            interest.Should().Be(175M);
        }

        [Fact]
        public void DodgyBankAccount_GetsBonusLumpSum()
        {
            var bankAccount = new DodgyBankAccount
            {
                Balance = 5000,
                InterestRate = 0.015M,
                BrownPaperBag = 10000M
            };
            var interest = CalculateInterest(bankAccount);
            interest.Should().Be(10075M);
        }

        private static decimal CalculateInterest(StandardBankAccount ba)
        {
            switch (ba)
            {
                case DodgyBankAccount dba:
                    return (dba.Balance * dba.InterestRate) + dba.BrownPaperBag;
                case SuperDuperBankAccount sdba:
                    return sdba.Balance * (sdba.BonusInterestRate + sdba.InterestRate);
                case StandardBankAccount sba when sba.Balance <= 10000:
                    return sba.Balance * sba.InterestRate;
                case StandardBankAccount sba:
                    return sba.Balance * (sba.InterestRate * 2);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
