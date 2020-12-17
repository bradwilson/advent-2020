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
	var board = new Board3();
	board.Init(data);
	// board.Print();

	for (var idx = 0; idx < 6; idx++)
	{
		board.Cycle();
		// board.Print();
	}

	return board.CubeCount;
}

long GetPart2()
{
	var board = new Board4();
	board.Init(data);
	// board.Print();

	for (var idx = 0; idx < 6; idx++)
	{
		board.Cycle();
		// board.Print();
	}

	return board.CubeCount;
}

var stopwatch = Stopwatch.StartNew();
Console.WriteLine($"Part 1: {GetPart1()}  ::  [{stopwatch.Elapsed}]");

stopwatch = Stopwatch.StartNew();
Console.WriteLine($"Part 2: {GetPart2()}  ::  [{stopwatch.Elapsed}]");

struct Range
{
	public int Low;

	public int High;

	public int Size => High - Low;

	public void Grow()
	{
		Low--;
		High++;
	}
}

class Board3
{
	HashSet<CubePoint> data = new();
	Range xRange;
	Range yRange;
	Range zRange;

	public int CubeCount => data.Count;

	int CountNeighbors(CubePoint point)
	{
		var result = 0;
		var neighbor = new CubePoint();

		for (neighbor.z = point.z - 1; neighbor.z <= point.z + 1; neighbor.z++)
			for (neighbor.y = point.y - 1; neighbor.y <= point.y + 1; neighbor.y++)
				for (neighbor.x = point.x - 1; neighbor.x <= point.x + 1; neighbor.x++)
				{
					if (neighbor.x == point.x && neighbor.y == point.y && neighbor.z == point.z)
						continue;

					if (data.Contains(neighbor))
						result++;
				}

		return result;
	}

	public void Cycle()
	{
		var newData = new HashSet<CubePoint>();
		var point = new CubePoint();

		for (point.z = zRange.Low - 1; point.z <= zRange.High; point.z++)
			for (point.y = yRange.Low - 1; point.y <= yRange.High; point.y++)
				for (point.x = xRange.Low - 1; point.x <= xRange.High; point.x++)
				{
					var neighbors = CountNeighbors(point);

					if (data.Contains(point))
					{
						if (neighbors == 2 || neighbors == 3)
							newData.Add(point);
					}
					else
					{
						if (neighbors == 3)
							newData.Add(point);
					}
				}

		xRange.Grow();
		yRange.Grow();
		zRange.Grow();
		data = newData;
	}

	public void Init(string[] lines)
	{
		xRange.High = lines.Length;
		yRange.High = lines.Length;
		zRange.High = 1;

		var y = 0;
		foreach (var line in lines)
		{
			var x = 0;
			foreach (var @char in line)
			{
				if (@char == '#')
					data.Add(new CubePoint { x = x, y = y, z = 0 });
				x++;
			}
			y++;
		}
	}

	public void Print()
	{
		var point = new CubePoint();

		Console.WriteLine($":: Board size (x,y,z) = {xRange.Size}x{yRange.Size}x{zRange.Size} ::");

		for (point.z = zRange.Low; point.z < zRange.High; point.z++)
		{
			Console.WriteLine($"z={point.z}");

			for (point.y = yRange.Low; point.y < yRange.High; point.y++)
			{
				for (point.x = xRange.Low; point.x < xRange.High; point.x++)
					Console.Write(data.Contains(point) ? '#' : '.');

				Console.WriteLine();
			}

			Console.WriteLine();
		}
	}

	struct CubePoint
	{
		public int x;
		public int y;
		public int z;
	}
}

class Board4
{
	HashSet<CubePoint> data = new();
	Range wRange;
	Range xRange;
	Range yRange;
	Range zRange;

	public int CubeCount => data.Count;

	int CountNeighbors(CubePoint point)
	{
		var result = 0;
		var neighbor = new CubePoint();

		for (neighbor.z = point.z - 1; neighbor.z <= point.z + 1; neighbor.z++)
			for (neighbor.y = point.y - 1; neighbor.y <= point.y + 1; neighbor.y++)
				for (neighbor.x = point.x - 1; neighbor.x <= point.x + 1; neighbor.x++)
					for (neighbor.w = point.w - 1; neighbor.w <= point.w + 1; neighbor.w++)
					{
						if (neighbor.w == point.w && neighbor.x == point.x && neighbor.y == point.y && neighbor.z == point.z)
							continue;

						if (data.Contains(neighbor))
							result++;
					}

		return result;
	}

	public void Cycle()
	{
		var newData = new HashSet<CubePoint>();
		var point = new CubePoint();

		for (point.z = zRange.Low - 1; point.z <= zRange.High; point.z++)
			for (point.y = yRange.Low - 1; point.y <= yRange.High; point.y++)
				for (point.x = xRange.Low - 1; point.x <= xRange.High; point.x++)
					for (point.w = wRange.Low - 1; point.w <= wRange.High; point.w++)
					{
						var neighbors = CountNeighbors(point);

						if (data.Contains(point))
						{
							if (neighbors == 2 || neighbors == 3)
								newData.Add(point);
						}
						else
						{
							if (neighbors == 3)
								newData.Add(point);
						}
					}

		wRange.Grow();
		xRange.Grow();
		yRange.Grow();
		zRange.Grow();
		data = newData;
	}

	public void Init(string[] lines)
	{
		wRange.High = 1;
		xRange.High = lines.Length;
		yRange.High = lines.Length;
		zRange.High = 1;

		var y = 0;
		foreach (var line in lines)
		{
			var x = 0;
			foreach (var @char in line)
			{
				if (@char == '#')
					data.Add(new CubePoint { w = 0, x = x, y = y, z = 0 });
				x++;
			}
			y++;
		}
	}

	public void Print()
	{
		var point = new CubePoint();

		Console.WriteLine($":: Board size (w,x,y,z) = {wRange.Size}x{xRange.Size}x{yRange.Size}x{zRange.Size} ::");

		for (point.w = wRange.Low; point.w < wRange.High; point.w++)
			for (point.z = zRange.Low; point.z < zRange.High; point.z++)
			{
				Console.WriteLine($"z={point.z}, w={point.w}");

				for (point.y = yRange.Low; point.y < yRange.High; point.y++)
				{
					for (point.x = xRange.Low; point.x < xRange.High; point.x++)
						Console.Write(data.Contains(point) ? '#' : '.');

					Console.WriteLine();
				}

				Console.WriteLine();
			}
	}

	struct CubePoint
	{
		public int w;
		public int x;
		public int y;
		public int z;
	}
}
