using System;
using System.IO;
using System.Linq;

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select line).ToArray();

(long nsAdder, long ewAdder) GetAdders(long direction) =>
	direction switch
	{
		0 => (0, 1),
		90 => (-1, 0),
		180 => (0, -1),
		270 => (1, 0),
		_ => throw new InvalidOperationException($"Unknown direction {direction}"),
	};

long GetPart1()
{
	var northSouth = 0L;  // South is negative
	var eastWest = 0L;    // West is negative
	var direction = 0L;   // 0 = east

	foreach (var line in data)
	{
		var action = line[0];
		var value = long.Parse(line[1..]);

		switch (line[0])
		{
			case 'F':
				var (nsAdder, ewAdder) = GetAdders(direction);
				northSouth += nsAdder * value;
				eastWest += ewAdder * value;
				break;
			case 'L':
				direction -= value;
				break;
			case 'R':
				direction += value;
				break;
			case 'N':
				northSouth += value;
				break;
			case 'S':
				northSouth -= value;
				break;
			case 'E':
				eastWest += value;
				break;
			case 'W':
				eastWest -= value;
				break;
			default:
				throw new InvalidOperationException($"Unknown action {action}");
		}

		while (direction < 0)
			direction += 360;
		while (direction >= 360)
			direction -= 360;
	}

	return Math.Abs(northSouth) + Math.Abs(eastWest);
}

long GetPart2()
{
	var nsShip = 0L;
	var ewShip = 0L;
	var nsWaypoint = 1L;
	var ewWaypoint = 10L;
	var direction = 0L;

	foreach (var line in data)
	{
		var action = line[0];
		var value = long.Parse(line[1..]);

		switch (line[0])
		{
			case 'F':
				nsShip += nsWaypoint * value;
				ewShip += ewWaypoint * value;
				break;
			case 'L':
				for (var i = 0L; i < value / 90; i++)
				{
					var newNS = ewWaypoint;
					var newEW = -nsWaypoint;
					nsWaypoint = newNS;
					ewWaypoint = newEW;
				}
				direction -= value;
				break;
			case 'R':
				for (var i = 0L; i < value / 90; i++)
				{
					var newNS = -ewWaypoint;
					var newEW = nsWaypoint;
					nsWaypoint = newNS;
					ewWaypoint = newEW;
				}
				direction += value;
				break;
			case 'N':
				nsWaypoint += value;
				break;
			case 'S':
				nsWaypoint -= value;
				break;
			case 'E':
				ewWaypoint += value;
				break;
			case 'W':
				ewWaypoint -= value;
				break;
			default:
				throw new InvalidOperationException($"Unknown action {action}");
		}

		while (direction < 0)
			direction += 360;
		while (direction >= 360)
			direction -= 360;
	}

	return Math.Abs(nsShip) + Math.Abs(ewShip);
}

Console.WriteLine($"Part 1: {GetPart1()}");
Console.WriteLine($"Part 2: {GetPart2()}");
