using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

var originalSeats =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select (new bool?[] { null }).Concat(from @char in line select @char == 'L' ? (bool?)false : null).Concat(new bool?[] { null }).ToList()
	).ToList();

var emptyRow = new bool?[originalSeats[0].Count].ToList();
originalSeats.Insert(0, emptyRow);
originalSeats.Add(emptyRow);

int CountPart1Neighbors(
	List<List<bool?>> board,
	int rowIndex,
	int colIndex)
{
	var result = 0;

	for (var ri = rowIndex - 1; ri < rowIndex + 2; ri++)
		for (var ci = colIndex - 1; ci < colIndex + 2; ci++)
			if ((ri != rowIndex || ci != colIndex) && board[ri][ci] == true)
				result++;

	return result;
}

int CountPart2Neighbors(
	List<List<bool?>> board,
	int rowIndex,
	int colIndex)
{
	var rowSize = board.Count;
	var colSize = board[0].Count;
	var result = 0;

	for (var rowShift = -1; rowShift < 2; rowShift++)
		for (var colShift = -1; colShift < 2; colShift++)
		{
			if (rowShift == 0 && colShift == 0)
				continue;

			var curRow = rowIndex;
			var curCol = colIndex;

			while (true)
			{
				curRow += rowShift;
				curCol += colShift;

				if (curRow == 0 || curRow == rowSize - 1 || curCol == 0 || curCol == colSize - 1 || board[curRow][curCol] == false)
					break;
				if (board[curRow][curCol] == true)
				{
					result++;
					break;
				}
			}
		}

	return result;
}

bool EqualBoards(
	List<List<bool?>> board1,
	List<List<bool?>> board2)
{
	for (var ri = 0; ri < board1.Count; ri++)
		for (var ci = 0; ci < board1[ri].Count; ci++)
			if (board1[ri][ci] != board2[ri][ci])
				return false;

	return true;
}

int GetFinalCount(
	List<List<bool?>> board,
	Func<List<List<bool?>>, int, int, int> neighborCounter,
	int occupiedTolerance)
{
	while (true)
	{
		Debugger.Break();

		var newSeats = new List<List<bool?>> { emptyRow };

		for (var rowIndex = 1; rowIndex < board.Count - 1; rowIndex++)
		{
			var row = board[rowIndex];
			var newRow = new List<bool?> { null };
			newSeats.Add(newRow);
			for (var colIndex = 1; colIndex < row.Count - 1; colIndex++)
			{
				if (row[colIndex] == null)
					newRow.Add(null);
				else if (row[colIndex] == false)
					newRow.Add(neighborCounter(board, rowIndex, colIndex) == 0);
				else
					newRow.Add(neighborCounter(board, rowIndex, colIndex) < occupiedTolerance);
			}
			newRow.Add(null);
		}

		newSeats.Add(emptyRow);

		if (EqualBoards(board, newSeats))
			return newSeats.Sum(r => r.Sum(c => c == true ? 1 : 0));

		board = newSeats;
	}
}

Console.WriteLine($"Part 1: {GetFinalCount(originalSeats, CountPart1Neighbors, 4)}");
Console.WriteLine($"Part 2: {GetFinalCount(originalSeats, CountPart2Neighbors, 5)}");
