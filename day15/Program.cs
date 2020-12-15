using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select line).ToArray();

long Speak(ref long turn, long number, Dictionary<long, long> whenSpoken)
{
	whenSpoken.TryGetValue(number, out var result);
	if (result != 0)
		result = turn - result;

	whenSpoken[number] = turn++;
	return result;
}

long GetPart(long stoppingPoint)
{
	var numbers = data[0].Split(",").Select(x => long.Parse(x)).ToList();
	var whenSpoken = new Dictionary<long, long>();
	var turn = 1L;
	var nextNumber = 0L;

	while (numbers.Count > 0)
	{
		nextNumber = Speak(ref turn, numbers[0], whenSpoken);
		numbers.RemoveAt(0);
	}

	while (turn < stoppingPoint)
		nextNumber = Speak(ref turn, nextNumber, whenSpoken);

	return nextNumber;
}

var stopwatch = Stopwatch.StartNew();
Console.WriteLine($"Part 1: {GetPart(2020L)}  ::  [{stopwatch.Elapsed}]");

stopwatch = Stopwatch.StartNew();
Console.WriteLine($"Part 2: {GetPart(30000000L)}  ::  [{stopwatch.Elapsed}]");
