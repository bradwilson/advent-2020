using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class Program
{
	static void Main()
	{
		var regex = new Regex(@"(\d+)-(\d+) (.): (.*)");
		var values =
			from line in File.ReadAllLines("input.txt")
			where !string.IsNullOrWhiteSpace(line)
			select line;
		var parsedValues =
			from value in values
			let match = regex.Match(value)
			where match.Success
			select (Lower: int.Parse(match.Groups[1].Value), Upper: int.Parse(match.Groups[2].Value), Char: match.Groups[3].Value[0], Value: match.Groups[4].Value);

		// Part 1
		var part1Valid =
			from parsedValue in parsedValues
			let charCount = parsedValue.Value.Count(c => c == parsedValue.Char)
			select charCount >= parsedValue.Lower && charCount <= parsedValue.Upper;
		Console.WriteLine($"Part 1: {part1Valid.Count(valid => valid)}");

		// Part 2
		var part2Valid =
			from parsedValue in parsedValues
			select parsedValue.Value[parsedValue.Lower - 1] == parsedValue.Char ^ parsedValue.Value[parsedValue.Upper - 1] == parsedValue.Char;
		Console.WriteLine($"Part 2: {part2Valid.Count(valid => valid)}");
	}
}
