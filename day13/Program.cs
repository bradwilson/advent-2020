using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select line).ToArray();

long GetPart1()
{
	var departureTime = long.Parse(data[0]);
	var part1BusIDs = data[1].Split(",").Where(id => id != "x").Select(id => long.Parse(id)).ToArray();

	var earliestArrivals = new Dictionary<long, long>();

	foreach (var busID in part1BusIDs)
	{
		var multiplier = (decimal)departureTime / busID;
		var intMultiplier = (long)Math.Ceiling(multiplier) * busID;
		earliestArrivals.Add(busID, intMultiplier);
	}

	var part1Bus = earliestArrivals.OrderBy(kvp => kvp.Value).First().Key;
	return part1Bus * (earliestArrivals[part1Bus] - departureTime);
}

long GetPart2()
{
	var busIDs = new List<(long busID, long offset)>();
	var offset = 0L;

	foreach (var busID in data[1].Split(","))
	{
		if (busID != "x")
			busIDs.Add((long.Parse(busID), offset));

		offset++;
	}

	// Algorithm is:
	//   - Start with the first bus. Use its schedule as our
	//     initial increment for verification. Remove it from
	//     the bus list.
	//   - Outer loop:
	//     - Loop over the remaining buses, and see if the current
	//       location also works for them.
	//       - If you have a match, then remove that bus from the
	//         list, and multiply its schedule into the increment
	//       - Ignore any bus which doesn't match
	//     - Out of buses? You're done. The current spot is the answer.
	//       Otherwise, move the schedule forward by the increment and repeat.

	// In practice, the offset of the first bus always seemed to be 0, but
	// this is a safer assumption. The data _might_ have started with "x".
	var currentStart = busIDs[0].busID - busIDs[0].offset;
	var increment = busIDs[0].busID;
	busIDs.RemoveAt(0);

	while (true)
	{
		var idx = 0;
		while (idx < busIDs.Count)
		{
			var targetPosition = currentStart + busIDs[idx].offset;
			if (targetPosition % busIDs[idx].busID == 0L)
			{
				increment *= busIDs[idx].busID;
				busIDs.RemoveAt(idx);
			}
			else
				idx++;
		}

		if (busIDs.Count == 0)
			return currentStart;

		currentStart += increment;
	}
}

Console.WriteLine($"Part 1: {GetPart1()}");
Console.WriteLine($"Part 2: {GetPart2()}");
