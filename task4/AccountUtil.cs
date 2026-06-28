using System;
using System.Collections.Generic;
using System.Security.Principal;
namespace mainTask;

public static class AccountUtil
{
    // Utility helper functions for Account class
    public static void Display(List<Account> accounts)
    {
        Console.WriteLine("\n=== Accounts ==========================================");
        foreach (var acc in accounts)
        {
            Console.WriteLine(acc);
        }
    }
    public static void Deposit(List<Account> accounts, double amount)
    {
        Console.WriteLine("\n=== Depositing to Accounts =================================");
        foreach (var acc in accounts)
        {
            if (acc.Deposit(amount))
                Console.WriteLine($"Deposited {amount} to {acc}");
            else
                Console.WriteLine($"Failed Deposit of {amount} to {acc}");
        }
    }
    public static void Withdraw(List<Account> accounts, double amount)
    {
        Console.WriteLine("\n=== Withdrawing from Accounts ==============================");
        foreach (var acc in accounts)
        {
            if (acc.Withdraw(amount))
                Console.WriteLine($"Withdrew {amount} from {acc}");
            else
                Console.WriteLine($"Failed Withdrawal of {amount} from {acc}");
        }
    }

    // Helper functions for Savings Account class
    // Helper functions for Checking Account class
    // Helper functions for Trust account class

    /*
     * 1) i can make any list as an account list so all errors will disapear
     */
}