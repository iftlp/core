using Shizima.CoreLib;

Console.WriteLine("Shizima.CoreLib.Client");

string value = "CoreLib";
string response = DebugCoreLib.Hello(value);
Console.WriteLine(response);