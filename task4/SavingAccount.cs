using System;
using System.Collections.Generic;
using System.Text;

namespace mainTask;

public class SavingsAccount : Account
{
    public double interest_rate { get; set; }
    public SavingsAccount(string name = "Unnamed Account", double balance = 0.0,double interest_rate = 1)
        :base(name,balance) 
    {
        this.interest_rate = interest_rate;
    }
    public override bool Deposit(double amount)
    {
        return base.Deposit(amount + (amount * (interest_rate / 100)));
    }
    public override string ToString()
    {
        return $"[Saving Account: {Name}: {Balance} : {interest_rate}]";
    }
}
