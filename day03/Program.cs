using System;
using System.IO;
using System.Linq;

var trees =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select
		(from @char in line
		 select @char == '#' ? true : false).ToArray()
	).ToArray();

var width = trees[0].Length;
var height = trees.Length;
long GetHitCount(int right, int down)
{
	var x = 0;
	var y = 0;
	var hitCount = 0L;
	while (y < height)
	{
		if (trees[y][x]) hitCount++;
		x = (x + right) % width;
		y += down;
	}
	return hitCount;
}

// Part 1
var threeOne = GetHitCount(3, 1);
Console.WriteLine($"Part 1: {threeOne}");

// Part 2
var oneOne = GetHitCount(1, 1);
var fiveOne = GetHitCount(5, 1);
var sevenOne = GetHitCount(7, 1);
var oneTwo = GetHitCount(1, 2);
Console.WriteLine($"Part 2: {oneOne * threeOne * fiveOne * sevenOne * oneTwo}");
