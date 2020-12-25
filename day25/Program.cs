using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select line).ToArray();

var subject1 = long.Parse(data[0]);
var subject2 = long.Parse(data[1]);

long GetLoopCount(long publicKey)
{
	var value = 1L;
	var loopSize = 0L;

	for (; value != publicKey; ++loopSize)
	{
		value *= 7L;
		value %= 20201227L;
	}

	return loopSize;
}

long Transform(long subject, long loopSize)
{
	var value = 1L;

	for (var idx = 0; idx < loopSize; ++idx)
	{
		value *= subject;
		value %= 20201227L;
	}

	return value;
}

long GetPart1()
{
	var subject2LoopCount = GetLoopCount(subject2);
	return Transform(subject1, subject2LoopCount);
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
var part1Result = GetPart1();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {part1Result}");
