using System;
using System.Collections.Generic;
using System.Text;

namespace mainTask
{
    public class CheckingAccount : Account
    {
        public double Fee { get; set; }
        public CheckingAccount(string name = "Unnamed Account", double balance = 0.0,double fee=1.5) 
            :base(name,balance)
        {
            Fee = fee;
        }

        public override bool Withdraw(double amount) => base.Withdraw(amount + Fee);
        public override string ToString()
        {
            return $"[Checking Account: {Name}: {Balance} : {Fee}]";
        }
    }
}
