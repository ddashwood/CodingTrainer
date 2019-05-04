string sInput;
int num1, num2;

// Get the user's input, turn it into a number, and store in num1
WriteLine("Enter the first number");
sInput = ReadLine();
num1 = int.Parse(sInput);

// Get the user's input, turn it into a number, and store in num2
WriteLine("Enter the second number");
sInput = ReadLine();
num2 = int.Parse(sInput);

WriteLine($"The answer is {num1 + num2}");
