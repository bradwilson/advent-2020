using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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

var stopwatch = Stopwatch.StartNew();
Console.WriteLine($"Part 1: {GetPart1()}  ::  [{stopwatch.Elapsed}]");

stopwatch = Stopwatch.StartNew();
Console.WriteLine($"Part 2: {GetPart2()}  ::  [{stopwatch.Elapsed}]");
