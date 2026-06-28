using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Text;

namespace mainTask;

public class TrustAccount : SavingsAccount
{
    //public int counter { get { return field; } set { field = value; } }
    //public int counter { get; set; }
    public int counter = 3;
    public TrustAccount(string name = "Unnamed Account", double balance = 0.0, double interes_rate = 0.1) 
                : base(name, balance, interes_rate) { }
    public override bool Deposit(double amount)
    {
        if (amount >= 5000)
            return base.Deposit(amount + 50);

        return base.Deposit(amount);
    }

    public override bool Withdraw(double amount)
    {
        if (counter > 0)
        {
            if (amount < (.2 * Balance)) 
            {
                return base.Withdraw(amount);
                counter = counter - 1;
            }
        }
        return false;
    }
    public override string ToString()
    {
        return $"[Checking Account: {Name}: {Balance} : {interest_rate}]";
    }  
    /* TO STRING ()
     * in fact we don't need to override this function here because it make the same thing as the parent of this clas
     * but i  don't know what instructor want so i put it  
    */
}
