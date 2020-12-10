using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var adapters =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select long.Parse(line)).Concat(new[] { 0L }).OrderBy(x => x).ToArray();

var gapOf1 = 0L;
var gapOf3 = 1L;

for (var idx = 0; idx < adapters.Length - 1; idx++)
{
	var gap = adapters[idx + 1] - adapters[idx];
	if (gap == 1)
		gapOf1++;
	else if (gap == 3)
		gapOf3++;
}

Console.WriteLine($"Part 1: {gapOf1 * gapOf3}");

var targetValue = adapters.Max() + 3;
var adapterHash = adapters.ToHashSet();
adapterHash.Add(targetValue);
var computedResults = new Dictionary<long, long>();

long CountPermutations(long currentValue)
{
	var permutations = 0L;

	if (computedResults.TryGetValue(currentValue, out var result))
		return result;

	if (currentValue == targetValue)
		return 1L;

	for (var step = 1; step < 4; step++)
		if (adapterHash.Contains(currentValue + step))
			permutations += CountPermutations(currentValue + step);

	computedResults[currentValue] = permutations;
	return permutations;
}

Console.WriteLine($"Part 2: {CountPermutations(0)}");
