using System;
using System.IO;
using System.Linq;

var instructions =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 let splitValues = line.Split(' ')
	 select (Instruction: splitValues[0], Value: long.Parse(splitValues[1]))).ToArray();

(long Value, bool InfiniteLoop) Accumulate()
{
	var visited = new bool[instructions.Length];
	var index = 0L;
	var accumulator = 0L;

	while (true)
	{
		if (index == instructions.Length)
			return (accumulator, false);

		if (index < 0 || index > instructions.Length)
			throw new InvalidOperationException($"Index out of range: {index} (0 - {instructions.Length - 1})");

		if (visited[index])
			return (accumulator, true);

		visited[index] = true;

		switch (instructions[index].Instruction)
		{
			case "nop":
				index++;
				break;

			case "acc":
				accumulator += instructions[index].Value;
				index++;
				break;

			case "jmp":
				index += instructions[index].Value;
				break;

			default:
				throw new InvalidOperationException($"Unknown instruction {instructions[index]}");
		}
	}
}

void FlipInstruction(long index)
{
	switch (instructions[index].Instruction)
	{
		case "nop":
			instructions[index].Instruction = "jmp";
			break;

		case "jmp":
			instructions[index].Instruction = "nop";
			break;

		default:
			break;
	}
}

long FixAndAccumulate()
{
	var changeIndex = 0L;

	while (true)
	{
		FlipInstruction(changeIndex);
		var (value, infiniteLoop) = Accumulate();
		if (!infiniteLoop)
			return value;
		FlipInstruction(changeIndex++);
	}
}

Console.WriteLine($"Part 1: {Accumulate().Value}");
Console.WriteLine($"Part 2: {FixAndAccumulate()}");
