using System;
using System.Collections.Generic;
using System.Linq;

namespace RecipeManagement.Core;

/// <summary>
/// Implement this class using the five Part A collections as private fields:
/// Dictionary&lt;int, Recipe&gt;, List&lt;string&gt;, LinkedList&lt;int&gt;,
/// Stack&lt;int&gt; and Queue&lt;string&gt;.
/// </summary>
public sealed class RecipeManager : IRecipeManager
{
    // TODO Part A: add your private collection fields here.
    private Dictionary<int, Recipe> recipes;
    private List<string> shoppingList;
    private LinkedList<int> cookingPlan;
    private Stack<int> removedRecipes;
    private Queue<string> instructions;
    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        // TODO Part A: validate recipes and build Dictionary<int, Recipe>.
        if (recipes == null)
        {
            throw new ArgumentNullException(nameof(recipes));
        }
        this.recipes = new Dictionary<int, Recipe>();
        foreach (var recipe in recipes)
        {
            if (recipe == null)
            {
                throw new ArgumentException("Recipe cannot be null.");
            }

            if (recipe.Id <= 0)
            {
                throw new ArgumentException("Recipe ID must be positive.");
            }

            if (string.IsNullOrWhiteSpace(recipe.Title))
            {
                throw new ArgumentException("Recipe title cannot be blank.");
            }

            if (this.recipes.ContainsKey(recipe.Id))
            {
                throw new ArgumentException("Duplicate recipe ID.");
            }
            this.recipes.Add(recipe.Id, recipe);
        }
        shoppingList = new List<string>();
        cookingPlan = new LinkedList<int>();
        removedRecipes = new Stack<int>();
        instructions = new Queue<string>();
    }

    public int RecipeCount => recipes.Count;
    public int ShoppingItemCount => shoppingList.Count;
    public int CookingPlanCount => cookingPlan.Count;
    public int PendingInstructionCount => instructions.Count;
    public int RemovedRecipeCount => removedRecipes.Count;

    public bool AddRecipe(Recipe recipe)
    {
        if (recipe == null)
        {
            throw new ArgumentNullException(nameof(recipe));
        }

        if (recipe.Id <= 0)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(recipe.Title))
        {
            return false;
        }

        if (recipes.ContainsKey(recipe.Id))
        {
            return false;
        }
       
        recipes.Add(recipe.Id, recipe);
        return true;
    }

    public Recipe? FindRecipe(int recipeId)
    {
        if (recipes.TryGetValue(recipeId, out Recipe? recipe))
        {
            return recipe;
        }
        return null;
    }

    public bool RemoveRecipe(int recipeId)
    {
        if (!recipes.ContainsKey(recipeId))
        {
            return false;
        }

        if (cookingPlan.Contains(recipeId))
        {
            return false;
        }
        recipes.Remove(recipeId);
        return true;
    }

    public int AddIngredientsToShoppingList(int recipeId)
    {
        if (!recipes.TryGetValue(recipeId, out Recipe? recipe))
        {
            return 0;
        }
        foreach (string ingredient in recipe.Ingredients)
        {
            shoppingList.Add(ingredient);
        }
        return recipe.Ingredients.Count;
    }
    public IReadOnlyList<string> GetShoppingList()
    {
        return new List<string>(shoppingList);
    }

    public void ClearShoppingList()
    {
        shoppingList.Clear();
    }
    public bool AddRecipeToCookingPlan(int recipeId)
    {
        if (!recipes.ContainsKey(recipeId))
        {
            return false;
        }

        if (cookingPlan.Contains(recipeId))
        {
            return false;
        }

        cookingPlan.AddLast(recipeId);
        return true;
    }

    public bool RemoveRecipeFromCookingPlan(int recipeId)
    {
        if (!cookingPlan.Contains(recipeId))
        {
            return false;
        }
        cookingPlan.Remove(recipeId);
        removedRecipes.Push(recipeId);
        return true;
    }

    public bool RestoreLastRemovedRecipe()
    {
        if (removedRecipes.Count == 0)
        {
            return false;
        }
        int recipeId = removedRecipes.Peek();

        if (!recipes.ContainsKey(recipeId))
        {
            return false;
        }

        if (cookingPlan.Contains(recipeId))
        {
            return false;
        }

        removedRecipes.Pop();
        cookingPlan.AddLast(recipeId);
        return true;
    }

    public int? PeekLastRemovedRecipe()
    {
        if (removedRecipes.Count == 0)
        {
            return null;
        }
        return removedRecipes.Peek();
    }

    public IReadOnlyList<int> GetCookingPlan()
    {
        return new List<int>(cookingPlan);
    }

    public bool StartCooking(int recipeId)
    {
        if (!recipes.TryGetValue(recipeId, out Recipe? recipe))
        {
            return false;
        }

        if (recipe.Instructions.Count == 0)
        {
            return false;
        }

        instructions.Clear();
        foreach (string instruction in recipe.Instructions)
        {
            instructions.Enqueue(instruction);
        }

        return true;
    }

    public string? PeekNextInstruction()
    {
        if (instructions.Count == 0)
        {
            return null;
        }
        return instructions.Peek();
    }

    public string? CompleteNextInstruction()
    {
        if (instructions.Count == 0)
        {
            return null;
        }
        return instructions.Dequeue();
    }

    public IReadOnlyList<Recipe> SearchByTitle(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByTitle.");

    public IReadOnlyList<Recipe> SearchByIngredient(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByIngredient.");

    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count) =>
        throw new NotImplementedException("Part B: implement GetHighestProteinRecipes.");

    public bool AddSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement AddSavedRecipe.");

    public bool RemoveSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement RemoveSavedRecipe.");

    public bool IsRecipeSaved(int recipeId) =>
        throw new NotImplementedException("Part B: implement IsRecipeSaved.");

    public IReadOnlyList<int> GetSavedRecipes() =>
        throw new NotImplementedException("Part B: implement GetSavedRecipes.");
}
