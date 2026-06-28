namespace Exception_dublicatNumber;

internal class Program
{
   
    static void Main(string[] args)
    {
        Console.WriteLine("Enter list of integres spearated by space : ");
        string numbers = Console.ReadLine();
        string[] IntergerList = numbers.Split(' ');

        //first way 

        //try
        //{
        //    IntergerList.Sort();
        //    for (int i = 0; i < IntergerList.Length - 1; i++)
        //    {
        //        if (IntergerList[i] == IntergerList[i + 1])
        //            throw new Exception("dublicate number");
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Console.WriteLine($"Message: {ex.Message}");

        //}


        //second way
        List<int> ints = new();
        foreach (string s in IntergerList)
        {
            ints.Add(Convert.ToInt32(s));
        }

        try
        {
            ints.Sort();
            for (int i = 0; i < ints.Count - 1; i++)
            {
                if (ints[i] == ints[i + 1])
                    throw new Exception("dublicate number");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Message: {ex.Message}");

        }




    }
}
