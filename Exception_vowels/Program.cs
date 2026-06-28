namespace Exception_vowels;

internal class Program
{
    static void Main(string[] args)
    {
        Console.Write("Enter a string: ");
        string input = Console.ReadLine().ToLower();
        try
        {
            if (input.Contains('a') == true || input.Contains('e') == true || input.Contains('o') == true || input.Contains('i') == true || input.Contains('y') == true)
                throw new Exception("have a vowel");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
