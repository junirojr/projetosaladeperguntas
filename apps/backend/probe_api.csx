using FastEndpoints.Testing;
using System.Reflection;

var testBaseType = typeof(TestBase<>);
Console.WriteLine($"TestBase<> members:");
foreach (var m in testBaseType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
{
    Console.WriteLine($"  {m.MemberType}: {m.Name}");
}
