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

long GetPart1()
{
	return 0L;
}

long GetPart2()
{
	return 0L;
}

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {GetPart1()}");

stopwatch = Stopwatch.StartNew();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {GetPart2()}");
