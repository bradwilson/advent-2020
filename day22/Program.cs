using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 select line.Trim()).ToArray();

long ComputeScore(List<int> values)
{
	var result = 0L;
	var totalCount = values.Count;
	for (var idx = 0; idx < totalCount; ++idx)
		result += (totalCount - idx) * (long)values[idx];

	return result;
}

long GetPart1()
{
	var player1 = new Queue<int>();
	var player2 = new Queue<int>();

	var idx = 1;

	for (; idx < data.Length && data[idx] != ""; ++idx)
		player1.Enqueue(int.Parse(data[idx]));

	for (idx += 2; idx < data.Length; ++idx)
		player2.Enqueue(int.Parse(data[idx]));

	while (player1.Count > 0 && player2.Count > 0)
	{
		var p1Card = player1.Dequeue();
		var p2Card = player2.Dequeue();

		if (p1Card > p2Card)
		{
			player1.Enqueue(p1Card);
			player1.Enqueue(p2Card);
		}
		else
		{
			player2.Enqueue(p2Card);
			player2.Enqueue(p1Card);
		}
	}

	var winner = player1.Count > 0 ? player1 : player2;
	return ComputeScore(winner.ToList());
}

long GetPart2()
{
	var player1 = new Queue<int>();
	var player2 = new Queue<int>();

	var idx = 1;

	for (; idx < data.Length && data[idx] != ""; ++idx)
		player1.Enqueue(int.Parse(data[idx]));

	for (idx += 2; idx < data.Length; ++idx)
		player2.Enqueue(int.Parse(data[idx]));

	(int winner, long score) RecursiveCombat(Queue<int> pl1, Queue<int> pl2)
	{
		var seenDecks = new HashSet<(string p1Hand, string p2Hand)>();
		while (true)
		{
			var currentDecks = (string.Join(",", pl1), string.Join(",", pl2));
			if (seenDecks.Contains(currentDecks))
				return (1, ComputeScore(pl1.ToList()));

			seenDecks.Add(currentDecks);

			var p1Card = pl1.Dequeue();
			var p2Card = pl2.Dequeue();
			int winner;

			if (pl1.Count >= p1Card && pl2.Count >= p2Card)
				(winner, _) = RecursiveCombat(new Queue<int>(pl1.Take(p1Card)), new Queue<int>(pl2.Take(p2Card)));
			else
				winner = p1Card > p2Card ? 1 : 2;

			if (winner == 1)
			{
				pl1.Enqueue(p1Card);
				pl1.Enqueue(p2Card);
			}
			else
			{
				pl2.Enqueue(p2Card);
				pl2.Enqueue(p1Card);
			}

			if (pl2.Count == 0)
				return (1, ComputeScore(pl1.ToList()));
			if (pl1.Count == 0)
				return (2, ComputeScore(pl2.ToList()));
		}
	}

	return RecursiveCombat(player1, player2).score;
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var p1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {p1Result}");

stopwatch = Stopwatch.StartNew();
var p2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {p2Result}");
