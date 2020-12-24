using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select line).ToArray();

var directions = new Dictionary<string, (long x, long y)>
{
	{ "se", (+1, +1) },
	{ "sw", (-1, +1) },
	{ "e", (+2, +0) },
	{ "w", (-2, +0) },
	{ "ne", (+1, -1) },
	{ "nw", (-1, -1) }
};

var tiles = new HashSet<(long x, long y)>();

long GetPart1()
{
	foreach (var instruction in data)
	{
		var current = (x: 0L, y: 0L);

		for (var idx = 0; idx < instruction.Length; ++idx)
		{
			string direction;

			if (instruction[idx] == 'n' || instruction[idx] == 's')
			{
				direction = instruction[idx..(idx + 2)];
				idx++;
			}
			else
				direction = instruction[idx..(idx + 1)];

			var (x, y) = directions[direction];

			current.x += x;
			current.y += y;
		}

		if (tiles.Contains(current))
			tiles.Remove(current);
		else
			tiles.Add(current);
	}

	return tiles.Count;
}

int CountBlackTileNeighbors(HashSet<(long x, long y)> toCount, long x, long y)
{
	var result = 0;

	foreach (var direction in directions.Values)
		if (toCount.Contains((x + direction.x, y + direction.y)))
			result++;

	return result;
}

long GetPart2()
{
	for (var day = 1; day <= 100; ++day)
	{
		var minX = tiles.Min(t => t.x);
		var maxX = tiles.Max(t => t.x);
		var minY = tiles.Min(t => t.y);
		var maxY = tiles.Max(t => t.y);
		var evenMinX = minX % 2 == 0 ? minX : minX + 1;
		var oddMinX = minX % 2 == 0 ? minX + 1 : minX;

		var newTiles = new HashSet<(long x, long y)>();

		for (var y = minY - 1; y <= maxY + 1; y++)
			for (var x = y % 2 == 0 ? evenMinX - 2 : oddMinX - 2; x <= maxX + 2; x += 2)
			{
				var current = (x, y);
				var currentIsBlack = tiles.Contains(current);
				var neighbors = CountBlackTileNeighbors(tiles, x, y);
				if (currentIsBlack)
				{
					if (neighbors == 0 || neighbors > 2)
						currentIsBlack = false;
				}
				else
				{
					if (neighbors == 2)
						currentIsBlack = true;
				}

				if (currentIsBlack)
					newTiles.Add(current);
			}

		tiles = newTiles;
	}

	return tiles.Count;
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");
