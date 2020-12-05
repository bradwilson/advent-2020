using System;
using System.IO;
using System.Linq;

var values =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select long.Parse(line)).ToArray();

Part1(values);
Part2(values);

static void Part1(long[] values)
{
	for (var first = 0; first < values.Length - 1; first++)
		for (var second = first + 1; second < values.Length; second++)
			if (values[first] + values[second] == 2020)
			{
				Console.WriteLine($"Part 1: {values[first] * values[second]}");
				return;
			}
}

static void Part2(long[] values)
{
	for (var first = 0; first < values.Length - 2; first++)
		for (var second = first + 1; second < values.Length - 1; second++)
			for (var third = second + 1; third < values.Length; third++)
				if (values[first] + values[second] + values[third] == 2020)
				{
					Console.WriteLine($"Part 2: {values[first] * values[second] * values[third]}");
					return;
				}
}
