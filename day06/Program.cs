using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var lines = File.ReadAllLines("input.txt");
var totalForAny = 0L;
var totalForAll = 0L;

for (var idx = 0; idx < lines.Length; ++idx)
{
	var groupAnswers = new Dictionary<char, int>();
	var personCount = 0;

	for (; idx < lines.Length; ++idx)
	{
		var line = lines[idx];

		if (string.IsNullOrWhiteSpace(line))
			break;

		personCount++;

		foreach (var @char in line)
		{
			if (!groupAnswers.ContainsKey(@char))
				groupAnswers[@char] = 1;
			else
				groupAnswers[@char]++;
		}
	}

	totalForAny += groupAnswers.Count;
	totalForAll += groupAnswers.Where(a => a.Value == personCount).Count();
}

Console.WriteLine($"Part 1: {totalForAny}");
Console.WriteLine($"Part 2: {totalForAll}");
