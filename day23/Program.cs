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

var cups = data[0].Select(x => int.Parse("" + x)).ToList();

void Move(LinkedList<int> cupsList, int iterations)
{
	var cupCount = cupsList.Count;
	var cupsByValue = new Dictionary<int, LinkedListNode<int>>();
	var currentNode = cupsList.First;

	while (currentNode != null)
	{
		cupsByValue.Add(currentNode.Value, currentNode);
		currentNode = currentNode.Next;
	}

	currentNode = cupsList.First;

	for (var move = 0; move < iterations; ++move)
	{
		var pickUps = new List<LinkedListNode<int>>();

		var currentCup = currentNode.Value;
		currentNode = currentNode.Next ?? cupsList.First;

		for (var pickupCount = 0; pickupCount < 3; pickupCount++)
		{
			var nextNode = currentNode.Next ?? cupsList.First;

			cupsList.Remove(currentNode);
			pickUps.Add(currentNode);

			currentNode = nextNode;
		}

		for (var destination = currentCup - 1; ; destination--)
		{
			if (destination == 0)
				destination = cupCount;

			if (!pickUps.Any(n => n.Value == destination))
			{
				var targetNode = cupsByValue[destination];
				foreach (var placedNode in pickUps.Reverse<LinkedListNode<int>>())
					cupsList.AddAfter(targetNode, placedNode);
				break;
			}
		}
	}
}

long GetPart1()
{
	var cupsList = new LinkedList<int>(cups);

	Move(cupsList, 100);

	var result = 0L;
	var node = cupsList.Find(1);
	for (var move = 1; move < 9; move++)
	{
		node = node.Next ?? cupsList.First;
		result = result * 10 + node.Value;
	}

	return result;
}

long GetPart2()
{
	var cupsList = new LinkedList<int>(cups.Concat(Enumerable.Range(10, 999_991)));

	Move(cupsList, 10_000_000);

	var cup1Node = cupsList.Find(1);
	var firstNode = cup1Node.Next ?? cupsList.First;
	var secondNode = firstNode.Next ?? cupsList.First;

	return ((long)firstNode.Value) * secondNode.Value;
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");

stopwatch = Stopwatch.StartNew();
var part2Result = GetPart2();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {part2Result}");
