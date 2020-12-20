using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 select line).ToArray();

var nameRegex = new Regex("^Tile (\\d+):$");
var tiles = new Dictionary<long, Tile>();

for (var idx = 0; idx < data.Length; ++idx)
{
	var match = nameRegex.Match(data[idx]);
	if (!match.Success)
		throw new InvalidOperationException($"Couldn't find tile name in '{data[idx]}'");

	var tileName = match.Groups[1].Value;

	var lines = new List<string>();
	for (++idx; idx < data.Length && data[idx] != ""; ++idx)
		lines.Add(data[idx]);

	var sides = new List<Side>();
	var sb = new StringBuilder();

	// top
	sides.Add(new Side { Text = lines[0] });

	// right
	foreach (var line in lines)
		sb.Append(line[^1]);
	sides.Add(new Side { Text = sb.ToString() });

	// bottom
	sides.Add(new Side { Text = Reverse(lines[^1]) });

	// left
	sb.Clear();
	foreach (var line in lines.Reverse<string>())
		sb.Append(line[0]);
	sides.Add(new Side { Text = sb.ToString() });

	var tileID = long.Parse(tileName);
	tiles.Add(tileID, new Tile { ID = tileID, Graph = lines, Sides = sides });
}

var sideSize = (int)Math.Sqrt(tiles.Count);

foreach (var tile in tiles)
{
	for (var sideIdx = 0; sideIdx < 4; sideIdx++)
	{
		var side = tile.Value.Sides[sideIdx];
		foreach (var otherTile in tiles.Where(t => t.Key != tile.Key))
		{
			for (var otherSideIdx = 0; otherSideIdx < 4; otherSideIdx++)
			{
				var otherSide = otherTile.Value.Sides[otherSideIdx];
				if (side.Text == otherSide.Text || side.Text == Reverse(otherSide.Text))
				{
					side.OtherTile = otherTile.Value;
					side.OtherSide = otherSide;
				}
			}
		}
	}
}

long GetPart1()
{
	var corners = tiles.Where(t => t.Value.Sides.Where(s => s.OtherTile != null).Count() == 2).ToList().ToList();
	return corners.Aggregate(1L, (acc, tile) => acc * tile.Key);
}

var four = new[] { 0, 1, 2, 3 };

long GetPart2()
{
	var mapping = new Tile[sideSize, sideSize];
	var corners = tiles.Where(t => t.Value.Sides.Where(s => s.OtherTile != null).Count() == 2).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
	var edges = tiles.Where(t => t.Value.Sides.Where(s => s.OtherTile != null).Count() == 3).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
	var centers = tiles.Where(t => t.Value.Sides.Where(s => s.OtherTile != null).Count() == 4).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

	var firstCorner = corners.First();
	var unmatchedSides = four.Where(x => firstCorner.Value.Sides[x].OtherTile == null).OrderBy(x => x).ToArray();
	var rotationCount = 4 - unmatchedSides[1];
	var tile = tiles[firstCorner.Key];
	for (var idx = 0; idx < rotationCount; ++idx)
		tile.Rotate();

	mapping[0, 0] = tile;

	var xIdx = 1;
	var yIdx = 0;

	while (yIdx < sideSize)
	{
		for (; xIdx < sideSize; xIdx++)
		{
			if (xIdx > 0)
			{
				// Match left edge of xIdx to right edge of xIdx-1
				var tileToMatch = mapping[xIdx - 1, yIdx];
				var sideToMatch = tileToMatch.Sides[1];
				var targetTile = sideToMatch.OtherTile;
				var targetSide = sideToMatch.OtherSide;
				var targetSideIdx = targetTile.Sides.IndexOf(targetSide);

				rotationCount = 3 - targetSideIdx;
				for (var idx = 0; idx < rotationCount; ++idx)
					targetTile.Rotate();

				if (targetSide.Text == sideToMatch.Text)
					targetTile.FlipHorizontal();

				mapping[xIdx, yIdx] = targetTile;
			}
			else
			{
				var tileToMatch = mapping[xIdx, yIdx - 1];
				var sideToMatch = tileToMatch.Sides[2];
				var targetTile = sideToMatch.OtherTile;
				var targetSide = sideToMatch.OtherSide;
				var targetSideIdx = targetTile.Sides.IndexOf(targetSide);

				rotationCount = (4 - targetSideIdx) % 4;
				for (var idx = 0; idx < rotationCount; ++idx)
					targetTile.Rotate();

				if (targetSide.Text == sideToMatch.Text)
					targetTile.FlipVertical();

				mapping[xIdx, yIdx] = targetTile;
			}
		}

		xIdx = 0;
		yIdx++;
	}

	var graphSideSize = mapping[0, 0].Graph.Count;
	var newLines = new List<string>();

	for (yIdx = 0; yIdx < sideSize; yIdx++)
	{
		for (var lineIndex = 1; lineIndex < graphSideSize - 1; lineIndex++)
		{
			var newLine = "";

			for (xIdx = 0; xIdx < sideSize; xIdx++)
			{
				tile = mapping[xIdx, yIdx];
				newLine += tile.Graph[lineIndex][1..^1];
			}

			newLines.Add(newLine);
		}
	}

	var topRegex = new Regex("^..................#.");
	var middleRegex = new Regex("#....##....##....###");
	var bottomRegex = new Regex("^.#..#..#..#..#..#...");

	for (var rotateCount = 0; rotateCount < 4; ++rotateCount)
	{
		for (var flipCount = 0; flipCount < 2; ++flipCount)
		{
			for (var middleIdx = 1; middleIdx < newLines.Count - 1; middleIdx++)
			{
				var middleMatch = middleRegex.Match(newLines[middleIdx]);
				if (middleMatch.Success)
				{
					var topMatch = topRegex.Match(newLines[middleIdx - 1][middleMatch.Index..]);
					var bottomMatch = bottomRegex.Match(newLines[middleIdx + 1][middleMatch.Index..]);
					if (topMatch.Success && bottomMatch.Success)
						return ReplaceMonstersAndCountWaves(newLines);
				}
			}

			// Flip

			var newGraph = new List<string>();
			foreach (var graphLine in newLines.Reverse<string>())
				newGraph.Add(graphLine);

			newLines = newGraph;
		}

		// Rotate

		var graphStacks = new StringBuilder[newLines[0].Length];
		for (var idx = 0; idx < newLines[0].Length; ++idx)
			graphStacks[idx] = new StringBuilder();

		foreach (var graphLine in newLines)
			for (var idx = 0; idx < graphLine.Length; ++idx)
				graphStacks[idx].Insert(0, graphLine[idx]);

		newLines = graphStacks.Select(sb => sb.ToString()).ToList();
	}

	return 0L;
}

long ReplaceMonstersAndCountWaves(List<string> newLines)
{
	var topPattern = "..................#.";
	var topRegex = new Regex($"^{topPattern}");
	var middlePattern = "#....##....##....###";
	var middleRegex = new Regex(middlePattern);
	var bottomPattern = ".#..#..#..#..#..#...";
	var bottomRegex = new Regex($"^{bottomPattern}");

	while (true)
	{
		var found = false;

		for (var middleIdx = 1; middleIdx < newLines.Count - 1; middleIdx++)
		{
			var middleMatch = middleRegex.Match(newLines[middleIdx]);
			if (middleMatch.Success)
			{
				var topMatch = topRegex.Match(newLines[middleIdx - 1][middleMatch.Index..]);
				var bottomMatch = bottomRegex.Match(newLines[middleIdx + 1][middleMatch.Index..]);
				if (topMatch.Success && bottomMatch.Success)
				{
					var newTopLine = newLines[middleIdx - 1][..middleMatch.Index];
					var newMidLine = newLines[middleIdx][..middleMatch.Index];
					var newBotLine = newLines[middleIdx + 1][..middleMatch.Index];

					for (var charIndex = 0; charIndex < topPattern.Length; charIndex++)
					{
						newTopLine += topPattern[charIndex] == '#' ? 'o' : newLines[middleIdx - 1][middleMatch.Index + charIndex];
						newMidLine += middlePattern[charIndex] == '#' ? 'o' : newLines[middleIdx][middleMatch.Index + charIndex];
						newBotLine += bottomPattern[charIndex] == '#' ? 'o' : newLines[middleIdx + 1][middleMatch.Index + charIndex];
					}

					newTopLine += newLines[middleIdx - 1][(middleMatch.Index + topPattern.Length)..];
					newMidLine += newLines[middleIdx][(middleMatch.Index + topPattern.Length)..];
					newBotLine += newLines[middleIdx + 1][(middleMatch.Index + topPattern.Length)..];

					newLines[middleIdx - 1] = newTopLine;
					newLines[middleIdx] = newMidLine;
					newLines[middleIdx + 1] = newBotLine;

					found = true;
					break;
				}
			}
		}

		if (!found)
			return newLines.Aggregate(0, (acc, line) => acc + line.Where(c => c == '#').Count());
	}
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {GetPart1()}");

stopwatch = Stopwatch.StartNew();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {GetPart2()}");

static string Reverse(string value)
{
	var sb = new StringBuilder();

	foreach (var ch in value.Reverse())
		sb.Append(ch);

	return sb.ToString();
}

class Tile
{
	public long ID;
	public List<string> Graph;
	public List<Side> Sides;

	public void FlipHorizontal()
	{
		var newGraph = new List<string>();
		foreach (var graphLine in Graph.Reverse<string>())
			newGraph.Add(graphLine);

		Sides[0].Text = ReverseString(Sides[0].Text);
		Sides[1].Text = ReverseString(Sides[1].Text);
		Sides[2].Text = ReverseString(Sides[2].Text);
		Sides[3].Text = ReverseString(Sides[3].Text);

		Sides = new List<Side> { Sides[2], Sides[1], Sides[0], Sides[3] };
		Graph = newGraph;
	}

	public void FlipVertical()
	{
		var newGraph = new List<string>();
		foreach (var graphLine in Graph)
			newGraph.Add(ReverseString(graphLine));

		Sides[0].Text = ReverseString(Sides[0].Text);
		Sides[1].Text = ReverseString(Sides[1].Text);
		Sides[2].Text = ReverseString(Sides[2].Text);
		Sides[3].Text = ReverseString(Sides[3].Text);

		Sides = new List<Side> { Sides[0], Sides[3], Sides[2], Sides[1] };
		Graph = newGraph;
	}

	public void Print()
	{
		Console.WriteLine($"Tile {ID}:");
		for (var idx = 0; idx < 4; ++idx)
			Console.WriteLine($"Edge {idx}: {Sides[idx].Text}");
		foreach (var graphLine in Graph)
			Console.WriteLine(graphLine);
		Console.WriteLine();
	}

	static string ReverseString(string value)
	{
		var sb = new StringBuilder();

		foreach (var ch in value.Reverse())
			sb.Append(ch);

		return sb.ToString();
	}

	public void Rotate()
	{
		var graphStacks = new StringBuilder[Graph[0].Length];
		for (var idx = 0; idx < Graph[0].Length; ++idx)
			graphStacks[idx] = new StringBuilder();

		foreach (var graphLine in Graph)
			for (var idx = 0; idx < graphLine.Length; ++idx)
				graphStacks[idx].Insert(0, graphLine[idx]);

		Sides = new List<Side> { Sides[3], Sides[0], Sides[1], Sides[2] };
		Graph = graphStacks.Select(sb => sb.ToString()).ToList();
	}
}

class Side
{
	public string Text;
	public Tile OtherTile;
	public Side OtherSide;
}
