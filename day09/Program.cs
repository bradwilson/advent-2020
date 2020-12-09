using System;
using System.IO;
using System.Linq;

var preambleSize = 25;

var numbers =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select long.Parse(line)).ToArray();

bool Part1IsValid(int index)
{
	for (var num1Index = index - preambleSize; num1Index < index - 1; num1Index++)
		for (var num2Index = num1Index + 1; num2Index < index; num2Index++)
			if (numbers[num1Index] + numbers[num2Index] == numbers[index])
				return true;

	return false;
}

long FindPart1()
{
	for (var index = preambleSize; index < numbers.Length; index++)
		if (!Part1IsValid(index))
			return numbers[index];

	throw new InvalidOperationException("Never found a bad number!");
}

var part1Answer = FindPart1();
Console.WriteLine($"Part 1: {part1Answer}");

long FindPart2()
{
	for (var index = 0; index < numbers.Length; index++)
	{
		var sum = 0L;
		for (var index2 = index; index2 < numbers.Length; index2++)
		{
			sum += numbers[index2];
			if (sum == part1Answer)
			{
				var subset = numbers[index..(index2 + 1)];
				var lowest = subset.Min();
				var highest = subset.Max();
				return lowest + highest;
			}
			if (sum > part1Answer)
				break;
		}
	}

	throw new InvalidOperationException("Couldn't find it!");
}

Console.WriteLine($"Part 2: {FindPart2()}");
