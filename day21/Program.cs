using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

var stopwatch = Stopwatch.StartNew();

var data =
	(from line in File.ReadAllLines("input.txt")
	 where !string.IsNullOrWhiteSpace(line)
	 select line.Trim()).ToArray();

var potentialIngredientsByAllergen = new Dictionary<string, List<HashSet<string>>>();
var recipes = new List<HashSet<string>>();

foreach (var line in data)
{
	var allergenListIdx = line.IndexOf('(');
	if (allergenListIdx < 0)
		throw new InvalidOperationException($"Line '{line}' doesn't have allergens");

	var ingredients = line[..allergenListIdx].Split(' ', StringSplitOptions.RemoveEmptyEntries).ToArray();
	recipes.Add(ingredients.ToHashSet());

	var allergenList = line[(allergenListIdx + "(contains ".Length)..^1];
	var allergens = allergenList.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
	foreach (var allergen in allergens)
	{
		if (!potentialIngredientsByAllergen.TryGetValue(allergen, out var listOfRecipes))
		{
			listOfRecipes = new List<HashSet<string>>();
			potentialIngredientsByAllergen[allergen] = listOfRecipes;
		}

		listOfRecipes.Add(ingredients.ToHashSet());
	}
}

(string allergen, string ingredient)? FindMatch(KeyValuePair<string, List<HashSet<string>>> kvp)
{
	var itemCount = kvp.Value.Count;
	var ingredientCounts = new Dictionary<string, int>();
	foreach (var ingredientList in kvp.Value)
		foreach (var ingredient in ingredientList)
			if (!ingredientCounts.ContainsKey(ingredient))
				ingredientCounts[ingredient] = 1;
			else
				ingredientCounts[ingredient]++;

	var ingredients = ingredientCounts.Where(i => i.Value == itemCount).ToList();
	if (ingredients.Count == 1)
		return (kvp.Key, ingredients[0].Key);

	return null;
}

var dangerousIngredients = new Dictionary<string, string>();

long GetPart1()
{
	var dict = potentialIngredientsByAllergen.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(x => x.ToArray().ToHashSet()).ToList());
	var recipesCopy = recipes.Select(x => x.ToArray().ToHashSet()).ToList();
	var unseenIngredients = potentialIngredientsByAllergen.SelectMany(kvp => kvp.Value).SelectMany(x => x).ToHashSet();

	while (dict.Count > 0)
	{
		foreach (var match in dict.OrderByDescending(kvp => kvp.Value.Count).Select(kvp => FindMatch(kvp)).Where(ic => ic != null))
		{
			dangerousIngredients.Add(match.Value.allergen, match.Value.ingredient);
			unseenIngredients.Remove(match.Value.ingredient);
			dict.Remove(match.Value.allergen);
			foreach (var kvp in dict)
				foreach (var list in kvp.Value)
					list.Remove(match.Value.ingredient);
			foreach (var recipe in recipesCopy)
				recipe.Remove(match.Value.ingredient);
		}
	}

	return recipesCopy.Sum(x => x.Count);
}

string GetPart2() =>
	string.Join(",", dangerousIngredients.OrderBy(x => x.Key).Select(x => x.Value));

Console.WriteLine($"[{stopwatch.Elapsed}] Pre-compute");

stopwatch = Stopwatch.StartNew();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 1: {GetPart1()}");

stopwatch = Stopwatch.StartNew();
Console.WriteLine($"[{stopwatch.Elapsed}] Part 2: {GetPart2()}");
