using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select line).ToArray();

long GetPart1()
{
	var memRegex = new Regex("^mem\\[(\\d+)\\] = (\\d+)$");
	var memory = new Dictionary<long, long>();
	var orMask = 0L;
	var andMask = -1L;

	foreach (var line in data)
	{
		if (line.StartsWith("mask = "))
		{
			orMask = 0L;
			andMask = 0L;

			var currentBit = 1L;
			foreach (var bit in line[7..].Reverse())
			{
				if (bit == 'X')
					andMask |= currentBit;
				else if (bit == '1')
					orMask |= currentBit;

				currentBit <<= 1;
			}
		}
		else
		{
			var match = memRegex.Match(line.Trim());
			if (!match.Success)
				throw new InvalidOperationException($"Unknown instruction: {line}");

			var address = long.Parse(match.Groups[1].Value);
			var value = long.Parse(match.Groups[2].Value);
			value = (value & andMask) | orMask;
			memory[address] = value;
		}
	}

	return memory.Values.Sum();
}

long GetPart2()
{
	var memRegex = new Regex("^mem\\[(\\d+)\\] = (\\d+)$");
	var memory = new Dictionary<long, long>();
	var orMask = 0L;
	var floatingBits = new List<long>();
	var floatingInstructions = new List<List<bool>>();

	foreach (var line in data)
	{
		if (line.StartsWith("mask = "))
		{
			var currentBit = 1L;
			orMask = 0L;
			floatingBits.Clear();
			floatingInstructions.Clear();

			foreach (var bit in line[7..].Reverse())
			{
				if (bit == 'X')
				{
					if (floatingInstructions.Count == 0)
					{
						floatingInstructions.Add(new List<bool> { false });
						floatingInstructions.Add(new List<bool> { true });
					}
					else
					{
						var newInstructions = new List<List<bool>>();
						foreach (var floatingInstruction in floatingInstructions)
						{
							newInstructions.Add(floatingInstruction.Append(false).ToList());
							newInstructions.Add(floatingInstruction.Append(true).ToList());
						}
						floatingInstructions = newInstructions;
					}

					floatingBits.Add(currentBit);
				}
				else if (bit == '1')
					orMask |= currentBit;

				currentBit <<= 1;
			}
		}
		else
		{
			var match = memRegex.Match(line.Trim());
			if (!match.Success)
				throw new InvalidOperationException($"Unknown instruction: {line}");

			var address = long.Parse(match.Groups[1].Value);
			var value = long.Parse(match.Groups[2].Value);

			foreach (var floatingInstruction in floatingInstructions)
			{
				var floatingOrMask = 0L;
				var floatingAndMask = 0xFFFFFFFFFL;

				for (var idx = 0; idx < floatingInstruction.Count; ++idx)
				{
					var bit = floatingBits[idx];
					if (floatingInstruction[idx])
						floatingOrMask |= bit;
					else
						floatingAndMask &= ~bit;
				}

				var newAddress = (address & floatingAndMask) | floatingOrMask | orMask;
				memory[newAddress] = value;
			}
		}
	}

	return memory.Values.Sum();
}

var stopwatch = Stopwatch.StartNew();
Console.WriteLine($"Part 1: {GetPart1()}  ::  [{stopwatch.Elapsed}]");

stopwatch = Stopwatch.StartNew();
Console.WriteLine($"Part 2: {GetPart2()}  ::  [{stopwatch.Elapsed}]");
