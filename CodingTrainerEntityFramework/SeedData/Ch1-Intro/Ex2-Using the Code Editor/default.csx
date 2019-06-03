WriteLine("Please enter a number");
string sNum = ReadLine();
if (int.TryParse(sNum, out int num))
{
    // Fix this line by adding 5 to the second number that is displayed
    WriteLine($"When I add 5 to {num}, I get {num}");
}
else
{
    WriteLine("That is not a number!");
}