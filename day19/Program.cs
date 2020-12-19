using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 select line).ToArray();

var rules = new Dictionary<int, HashSet<string>>();
var ruleLines = new Dictionary<int, (string text, HashSet<int> values)>();
var dataIdx = 0;

for (; data[dataIdx] != ""; ++dataIdx)
{
	var split = data[dataIdx].Split(':', 2);
	var ruleNumber = int.Parse(split[0]);
	var ruleText = split[1].Trim();
	if (ruleText[0] == '"')
		rules[ruleNumber] = new HashSet<string> { ruleText[1..2] };
	else
	{
		split = ruleText.Split(' ').Where(x => x != "|").ToArray();
		ruleLines[ruleNumber] = (ruleText, split.Select(x => int.Parse(x)).ToHashSet());
	}
}

IEnumerable<string> GetValue(int[] indices)
{
	if (indices.Length == 1)
		return rules[indices[0]];

	var result = new List<string>();
	var otherIndices = indices[1..];
	foreach (var value in rules[indices[0]])
		foreach (var otherValue in GetValue(otherIndices))
			result.Add(value + otherValue);

	return result;
}

while (ruleLines.Count > 0)
{
	var ruleLine = ruleLines.First(x => x.Value.values.All(y => rules.ContainsKey(y)));
	var ruleValues = new HashSet<string>();

	var split = ruleLine.Value.text.Split('|');
	foreach (var splitPiece in split)
		foreach (var result in GetValue(splitPiece.Trim().Split(' ').Select(x => int.Parse(x)).ToArray()))
			ruleValues.Add(result);

	rules[ruleLine.Key] = ruleValues;
	ruleLines.Remove(ruleLine.Key);
}

var potentials = data[(dataIdx + 1)..];

long GetPart1()
{
	var matchers = rules[0];
	return potentials.Count(x => matchers.Contains(x));
}

long GetPart2()
{
	var matchCount = 0L;
	var firstHalfMatches = new List<(string residual, int countOf31)>();

	// New rules are:
	//   8: 42 | 42 8
	//   11: 42 31 | 42 11 31
	// And our root rule is unchanged:
	//   0: 8 11
	// Combined this means:
	//   - The string needs to end with at least one 31 match
	//   - The string needs to start with at least one more 42 matches than 31 matches

	foreach (var potential in potentials)
	{
		var countOf31 = 0;
		var residual = potential;

		while (true)
		{
			var matching31 = rules[31].SingleOrDefault(x => residual.EndsWith(x));
			if (matching31 == null)
				break;

			residual = residual[..(residual.Length - matching31.Length)];
			countOf31++;
		}

		if (countOf31 > 0)
			firstHalfMatches.Add((residual, countOf31));
	}

	foreach (var potential in firstHalfMatches)
	{
		var countOf42 = 0;
		var residual = potential.residual;

		while (true)
		{
			var matching42 = rules[42].SingleOrDefault(x => residual.StartsWith(x));
			if (matching42 == null)
				break;

			residual = residual[matching42.Length..];
			countOf42++;
		}

		if (residual == "" && countOf42 > potential.countOf31)
			matchCount++;
	}

	return matchCount;
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {GetPart1()}");

stopwatch = Stopwatch.StartNew();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {GetPart2()}");
