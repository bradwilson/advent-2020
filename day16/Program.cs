using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var fieldRegex = new Regex("^(.*): (\\d+)-(\\d+) or (\\d+)-(\\d+)$");

var data =
	(from line in File.ReadAllLines("input.txt")
	 select line).ToArray();

var fields = new List<Field>();
var idx = 0;

for (; data[idx] != string.Empty; ++idx)
{
	var match = fieldRegex.Match(data[idx]);
	if (!match.Success)
		throw new InvalidOperationException($"Could not regex: '{data[idx]}'");

	fields.Add(new Field { name = match.Groups[1].Value, first = (int.Parse(match.Groups[2].Value), int.Parse(match.Groups[3].Value)), second = (int.Parse(match.Groups[4].Value), int.Parse(match.Groups[5].Value)) });
}

if (data[++idx] != "your ticket:")
	throw new InvalidOperationException("Couldn't find my ticket");

var myTicket = data[++idx].Split(",").Select(x => int.Parse(x)).ToList();

if (data[++idx] != "")
	throw new InvalidOperationException("Missing separator");

if (data[++idx] != "nearby tickets:")
	throw new InvalidOperationException("Couldn't find nearby tickets");

var nearbyTickets = new List<List<int>>();

for (++idx; idx < data.Length; ++idx)
	nearbyTickets.Add(data[idx].Split(",").Select(x => int.Parse(x)).ToList());

var validTickets = new List<List<int>>();

long GetPart1()
{
	var result = 0L;

	foreach (var nearbyTicket in nearbyTickets)
	{
		var validTicket = true;

		foreach (var fieldValue in nearbyTicket)
		{
			var found = false;

			foreach (var field in fields)
			{
				if ((fieldValue >= field.first.lower && fieldValue <= field.first.upper) ||
					(fieldValue >= field.second.lower && fieldValue <= field.second.upper))
				{
					found = true;
					break;
				}
			}

			if (!found)
			{
				result += fieldValue;
				validTicket = false;
				break;
			}
		}

		if (validTicket)
			validTickets.Add(nearbyTicket);
	}

	return result;
}

long GetPart2()
{
	var validPositionsByField = new List<(Field, HashSet<int>)>();

	for (var fieldIndex = 0; fieldIndex < fields.Count; ++fieldIndex)
	{
		var field = fields[fieldIndex];
		var validPositions = new HashSet<int>(Enumerable.Range(0, fields.Count));

		for (var validTicketIndex = 0; validTicketIndex < validTickets.Count; ++validTicketIndex)
		{
			var validTicket = validTickets[validTicketIndex];
			foreach (var validPosition in validPositions.ToList())
			{
				var fieldValue = validTicket[validPosition];
				if (!((fieldValue >= field.first.lower && fieldValue <= field.first.upper) ||
					  (fieldValue >= field.second.lower && fieldValue <= field.second.upper)))
				{
					validPositions.Remove(validPosition);
				}
			}
		}

		if (validPositions.Count == 0)
			throw new InvalidOperationException($"Rejected all values for field {field.name}");

		validPositionsByField.Add((field, validPositions));
	}

	var positionsByName = new Dictionary<string, int>();

	while (validPositionsByField.Count > 0)
	{
		var validPositionByField = validPositionsByField.First(x => x.Item2.Count == 1);
		var position = validPositionByField.Item2.Single();

		positionsByName[validPositionByField.Item1.name] = position;
		validPositionsByField.Remove(validPositionByField);
		foreach (var otherField in validPositionsByField)
			otherField.Item2.Remove(position);
	}

	var result = 1L;

	foreach (var position in positionsByName.Where(x => x.Key.StartsWith("departure")))
		result *= myTicket[position.Value];

	return result;
}

var stopwatch = Stopwatch.StartNew();
Console.WriteLine($"Part 1: {GetPart1()}  ::  [{stopwatch.Elapsed}]");

stopwatch = Stopwatch.StartNew();
Console.WriteLine($"Part 2: {GetPart2()}  ::  [{stopwatch.Elapsed}]");

public class Field
{
	public string name;
	public (int lower, int upper) first;
	public (int lower, int upper) second;
}
