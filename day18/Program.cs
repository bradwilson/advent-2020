using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select line).ToArray();

long GetResult(string line, bool precedence)
{
	var isDigits = true;
	char? operation = null;
	var stack = new Stack<long>();
	var toParse = line;

	while (!string.IsNullOrEmpty(toParse))
	{
		if (toParse[0] == '(')
		{
			var parenCount = 1;

			for (var idx = 1; idx < toParse.Length; ++idx)
			{
				if (toParse[idx] == '(')
					parenCount++;
				else if (toParse[idx] == ')')
				{
					if (--parenCount == 0)
					{
						var innerExpression = toParse[1..idx];
						var innerValue = GetResult(innerExpression, precedence);
						stack.Push(innerValue);

						if (operation != null)
						{
							if (operation == '+')
								stack.Push(stack.Pop() + stack.Pop());
							else if (!precedence)
								stack.Push(stack.Pop() * stack.Pop());

							operation = null;
						}

						toParse = toParse[(idx + 1)..].Trim();
						break;
					}
				}
			}

			isDigits = false;
		}
		else if (isDigits)
		{
			var splitPieces = toParse.Split(' ', 2);
			stack.Push(long.Parse(splitPieces[0]));

			if (operation != null)
			{
				if (operation == '+')
					stack.Push(stack.Pop() + stack.Pop());
				else if (!precedence)
					stack.Push(stack.Pop() * stack.Pop());

				operation = null;
			}

			if (splitPieces.Length == 1)
				break;

			toParse = splitPieces[1];
			isDigits = false;
		}
		else
		{
			operation = toParse[0];
			toParse = toParse[1..].Trim();
			isDigits = true;
		}
	}

	var result = 1L;
	while (stack.Count != 0)
		result *= stack.Pop();

	return result;
}

long GetPart1()
{
	var result = 0L;

	foreach (var line in data)
		result += GetResult(line, false);

	return result;
}

long GetPart2()
{
	var result = 0L;

	foreach (var line in data)
		result += GetResult(line, true);

	return result;
}

var stopwatch = Stopwatch.StartNew();
Console.WriteLine($"Part 1: {GetPart1()}  ::  [{stopwatch.Elapsed}]");

stopwatch = Stopwatch.StartNew();
Console.WriteLine($"Part 2: {GetPart2()}  ::  [{stopwatch.Elapsed}]");
