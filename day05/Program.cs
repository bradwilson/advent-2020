using System;
using System.IO;
using System.Linq;

long GetSeatID(string partition)
{
	long low = 0;
	long high = 127;

	for (int idx = 0; idx < 7; idx++)
		if (partition[idx] == 'F')
			high = (low + high) / 2;
		else
			low = (low + high) / 2 + 1;

	var row = low;

	low = 0;
	high = 7;

	for (int idx = 7; idx < 10; idx++)
		if (partition[idx] == 'L')
			high = (low + high) / 2;
		else
			low = (low + high) / 2 + 1;

	var col = low;

	return row * 8 + col;
}

var lines =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select line).ToArray();

var seatIDs = lines.Select(GetSeatID).OrderBy(x => x).ToArray();

Console.WriteLine($"Part 1: {seatIDs.Max()}");

for (int idx = 1; idx < seatIDs.Length - 1; idx++)
	if (seatIDs[idx - 1] != seatIDs[idx] - 1)
	{
		Console.WriteLine($"Part 2: {seatIDs[idx] - 1}");
		break;
	}
