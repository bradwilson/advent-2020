using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

// 0: ([faded blue] bags contain) (no other bags).
// >0: ([vibrant plum] bags contain) (5 [faded blue] bags, 6 [dotted black] bags).

var outerRegex = new Regex("^(.*) bags contain (.*).$");
var innerRegex = new Regex("(\\d+) (.*) bag");
var rules =
	from line in File.ReadAllLines("input.txt")
	where !string.IsNullOrWhiteSpace(line)
	select line.Trim();
var containingColorsByColor = new Dictionary<string, HashSet<string>>();
var colorAndCountByContainingColor = new Dictionary<string, List<(int count, string color)>>();

foreach (var rule in rules)
{
	var outerMatch = outerRegex.Match(rule);
	if (!outerMatch.Success)
	{
		Console.WriteLine($"Didn't match our outer regex: {rule}");
		continue;
	}

	var containingColor = outerMatch.Groups[1].Value;
	var innerBags = outerMatch.Groups[2].Value;

	if (innerBags != "no other bags")
	{
		foreach (var containedText in innerBags.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
		{
			var innerMatch = innerRegex.Match(containedText);
			if (!innerMatch.Success)
			{
				Console.WriteLine($"Didn't match our inner regex: {containedText}");
				continue;
			}

			// Part 1 parsing
			var containedColor = innerMatch.Groups[2].Value;
			if (!containingColorsByColor.ContainsKey(containedColor))
				containingColorsByColor.Add(containedColor, new HashSet<string>());

			containingColorsByColor[containedColor].Add(containingColor);

			// Part 2 parsing
			if (!colorAndCountByContainingColor.ContainsKey(containingColor))
				colorAndCountByContainingColor.Add(containingColor, new List<(int count, string color)>());

			var count = int.Parse(innerMatch.Groups[1].Value);
			colorAndCountByContainingColor[containingColor].Add((count, containedColor));
		}
	}
}

void AddToPart1Answers(HashSet<string> answers, ICollection<string> containers)
{
	foreach (var container in containers)
	{
		answers.Add(container);
		if (containingColorsByColor.TryGetValue(container, out var innerContainers))
			AddToPart1Answers(answers, innerContainers);
	}
}

var part1Answers = new HashSet<string>();
AddToPart1Answers(part1Answers, containingColorsByColor["shiny gold"]);
Console.WriteLine($"Part 1: {part1Answers.Count}");

long CountBags(string color)
{
	var result = 1L;

	if (colorAndCountByContainingColor.TryGetValue(color, out var containedColors))
		foreach (var containedColor in containedColors)
			result += containedColor.count * CountBags(containedColor.color);

	return result;
}

Console.WriteLine($"Part 2: {CountBags("shiny gold") - 1}");
