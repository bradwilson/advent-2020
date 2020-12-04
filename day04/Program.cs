using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var eclValidValues = new HashSet<string> { "amb", "blu", "brn", "gry", "grn", "hzl", "oth" };
var hclRegex = new Regex("^#[0-9a-f]{6}$");
var pidRegex = new Regex("^[0-9]{9}$");
var requiredFields = new Dictionary<string, Func<string, bool>>
{
	{ "byr", ValidateByr },
	{ "iyr", ValidateIyr },
	{ "eyr", ValidateEyr },
	{ "hgt", ValidateHgt },
	{ "hcl", ValidateHcl },
	{ "ecl", ValidateEcl },
	{ "pid", ValidatePid },
};

bool ValidateByr(string value) =>
	value.Length == 4 &&
	int.TryParse(value, out var intValue) &&
	intValue >= 1920 && intValue <= 2002;

bool ValidateIyr(string value) =>
	value.Length == 4 &&
	int.TryParse(value, out var intValue) &&
	intValue >= 2010 && intValue <= 2020;

bool ValidateEyr(string value) =>
	value.Length == 4 &&
	int.TryParse(value, out var intValue) &&
	intValue >= 2020 && intValue <= 2030;

bool ValidateHgt(string value)
{
	if (value.EndsWith("cm"))
	{
		if (int.TryParse(value[0..^2], out var intValue))
			return intValue >= 150 && intValue <= 193;
	}
	else if (value.EndsWith("in"))
	{
		if (int.TryParse(value[0..^2], out var intValue))
			return intValue >= 59 && intValue <= 76;
	}

	return false;
}

bool ValidateHcl(string value) =>
	hclRegex.Match(value).Success;

bool ValidateEcl(string value) =>
	eclValidValues.Contains(value);

bool ValidatePid(string value) =>
	pidRegex.Match(value).Success;

var lines = File.ReadAllLines("input.txt");
var part1Valid = 0;
var part2Valid = 0;
var idx = 0;

while (true)
{
	var fields = new Dictionary<string, string>();

	while (true)
	{
		var line = lines[idx++];
		if (string.IsNullOrWhiteSpace(line))
			break;

		var rawFields = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
		foreach (var rawField in rawFields)
		{
			var rawFieldValues = rawField.Split(":");
			fields.Add(rawFieldValues[0], rawFieldValues[1]);
		}

		if (idx == lines.Length)
			break;
	}

	if (requiredFields.Keys.All(field => fields.Keys.Contains(field)))
	{
		part1Valid++;

		if (requiredFields.All(kvp => kvp.Value(fields[kvp.Key])))
			part2Valid++;
	}

	if (idx == lines.Length)
		break;
}

Console.WriteLine($"Part 1: {part1Valid}");
Console.WriteLine($"Part 2: {part2Valid}");
