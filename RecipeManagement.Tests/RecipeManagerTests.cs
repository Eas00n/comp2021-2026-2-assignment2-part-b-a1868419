using System.Collections.Generic;
using System.Net.Mail;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Example tests from the assignment specification. Add your own tests as you work.
/// </summary>
public sealed class RecipeManagerTests
{
    [Fact]
    public void Constructor_BuildsRecipeDictionary()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);
        Assert.Equal("Recipe A", manager.FindRecipe(10)?.Title);
    }

    [Fact]
    public void InstructionsAreCompletedInFileOrder()
    {
        var manager = CreateManager();
        Assert.True(manager.StartCooking(10));
        Assert.Equal("First step", manager.PeekNextInstruction());
        Assert.Equal("First step", manager.CompleteNextInstruction());
        Assert.Equal("Second step", manager.PeekNextInstruction());
    }

    [Fact]
    public void RemovedRecipesAreRestoredLastInFirstOut()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(20);
        Assert.Equal(20, manager.PeekLastRemovedRecipe());
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());
    }

    private static RecipeManager CreateManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 10,
                Title = "Recipe A",
                Ingredients = new() { "1 apple" },
                Instructions = new() { "First step", "Second step" }
            },
            new Recipe
            {
                Id = 20,
                Title = "Recipe B"
            }
        });
    }

    // My own tests
    //Tests that a recipe can be added to the Dictionary, found by its ID, and then removed successfully.
    [Fact]
    public void DictionaryCanAddFindAndRemoveRecipe()
    {
        var manager = CreateManager();
        var recipe = new Recipe
        {
            Id = 398654,
            Title = "Recipe C",

        };

        Assert.True(manager.AddRecipe(recipe));
        Assert.Equal(recipe, manager.FindRecipe(398654));
        Assert.True(manager.RemoveRecipe(398654));
        Assert.Null(manager.FindRecipe(398654));
    }

    // Tests that the Dictionary prevents adding a recipe with a duplicate recipe ID.
    [Fact]
    public void CannotAddDuplicateRecipeId()
    {
        var manager = CreateManager();
        var recipe = new Recipe
        {
            Id = 398654,
            Title = "Duplicate Recipe"
        };
        Assert.True(manager.AddRecipe(recipe));
        Assert.False(manager.AddRecipe(recipe));
        Assert.Equal(3, manager.RecipeCount);
        Assert.Equal("Duplicate Recipe", manager.FindRecipe(398654)?.Title);
    }

    //Tests that searching for a missing recipe ID returns null and does not remove any recipe.
    [Fact]
    public void MissingRecipeIdIsHandled()
    {
        var manager = CreateManager();

        Assert.Null(manager.FindRecipe(50));
        Assert.False(manager.RemoveRecipe(50));
        Assert.Equal(2, manager.RecipeCount);
    }

    //Tests that ingredients are added to the List in the correct order and that the shopping list can be cleared.
    [Fact]
    public void ShoppingListCanAddGetAndClearIngredients()
    {
        var manager = CreateManager();

        Assert.Equal(1, manager.AddIngredientsToShoppingList(10));
        Assert.Equal(1, manager.ShoppingItemCount);
        Assert.Equal(new[] { "1 apple" }, manager.GetShoppingList());

        manager.ClearShoppingList();
        Assert.Empty(manager.GetShoppingList());
        Assert.Equal(0, manager.ShoppingItemCount);
    }

    //Tests that recipes can be added to the LinkedList, retrieved in order, and removed successfully.
    [Fact]
    public void CookingPlanCanAddGetAndRemoveRecipes()
    {
        var manager = CreateManager();

        Assert.True(manager.AddRecipeToCookingPlan(10));
        Assert.True(manager.AddRecipeToCookingPlan(20));
        Assert.Equal(2, manager.CookingPlanCount);
        Assert.Equal(new[] { 10, 20 }, manager.GetCookingPlan());
        Assert.True(manager.RemoveRecipeFromCookingPlan(10));
        Assert.Equal(1, manager.CookingPlanCount);
        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());
    }

    //Tests that the Cooking Plan prevents the same recipe from being added more than once.
    [Fact]
    public void CannotAddDuplicateRecipeToCookingPlan()
    {
        var manager = CreateManager();

        Assert.True(manager.AddRecipeToCookingPlan(20));
        Assert.False(manager.AddRecipeToCookingPlan(20));
        Assert.Equal(1, manager.CookingPlanCount);
        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());
    }

    //Tests the behaviour of the Stack when it is empty. Peek and Restore should return safe results without errors.
    [Fact]
    public void EmptyRemovedRecipeStackIsHandled()
    {
        var manager = CreateManager();

        Assert.Equal(0, manager.RemovedRecipeCount);
        Assert.Null(manager.PeekLastRemovedRecipe());
        Assert.False(manager.RestoreLastRemovedRecipe());
    }

    //Tests that the Queue processes cooking instructions in first-in-first-out (FIFO) order.
    [Fact]
    public void CookingInstructionsUseQueueInOrder()
    {
        var manager = CreateManager();

        Assert.True(manager.StartCooking(10));
        Assert.Equal(2, manager.PendingInstructionCount);
        Assert.Equal("First step", manager.PeekNextInstruction());
        Assert.Equal("First step", manager.CompleteNextInstruction());
        Assert.Equal(1, manager.PendingInstructionCount);
        Assert.Equal("Second step", manager.CompleteNextInstruction());
        Assert.Equal(0, manager.PendingInstructionCount);
    }

    //Tests the behaviour of an empty instruction Queue. Peek and Complete should return null when there are no instructions.
    [Fact]
    public void EmptyInstructionQueueIsHandled()
    {
        var manager = CreateManager();

        Assert.Equal(0, manager.PendingInstructionCount);
        Assert.Null(manager.PeekNextInstruction());
        Assert.Null(manager.CompleteNextInstruction());
    }

    //Tests that removed recipes are stored in the Stack and restored in last-in-first-out (LIFO) order.
    [Fact]
    public void RemovedRecipesUseLastInFirstOut()
    {
        var manager = CreateManager();

        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);

        Assert.True(manager.RemoveRecipeFromCookingPlan(10));
        Assert.True(manager.RemoveRecipeFromCookingPlan(20));

        Assert.Equal(2, manager.RemovedRecipeCount);
        Assert.Equal(20, manager.PeekLastRemovedRecipe());
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(1, manager.RemovedRecipeCount);
        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());
    }

    //Tests that recipe instructions can be obtained from the recipe catalogue and loaded into the cooking Queue.
    [Fact]
    public void RecipeInstructionsCanBeLoadedFromCatalogue()
    {
        var manager = CreateManager();

        Assert.Equal("Recipe A", manager.FindRecipe(10)?.Title);
        Assert.True(manager.StartCooking(10));
        Assert.Equal("First step", manager.PeekNextInstruction());
    }

    //Part B
    //Test add title search coverage.
    [Fact]
    public void SearchByTitle_ShouldReturnMatchingRecipes()
    {
        var manager = CreateManager();

        var results = manager.SearchByTitle("recipe");
        Assert.Equal(2, results.Count);
        Assert.Contains(results, recipe => recipe.Title == "Recipe A");
        Assert.Contains(results, recipe => recipe.Title == "Recipe B");
    }

    //Tests that title search supports partial matching.
    [Fact]
    public void SearchByTitle_ShouldSupportPartialMatching()
    {
        var manager = CreateManager();

        var results = manager.SearchByTitle("A");
        Assert.Single(results);
        Assert.Equal("Recipe A", results[0].Title);
    }

    //Tests that a blank title search returns all recipes.
    [Fact]
    public void SearchByTitle_ShouldReturnAllRecipesForBlankSearch()
    {
        var manager = CreateManager();

        var results = manager.SearchByTitle("");
        Assert.Equal(2, results.Count);
    }

    //Tests that ingredient search returns recipes containing the requested ingredient.
    [Fact]
    public void SearchByIngredient_ShouldReturnMatchingRecipes()
    {
        var manager = CreateManager();

        var results = manager.SearchByIngredient("apple");
        Assert.Single(results);
        Assert.Equal("Recipe A", results[0].Title);
    }

    //Tests that ingredient search is case-insensitive.
    [Fact]
    public void SearchByIngredient_ShouldBeCaseInsensitive()
    {
        var manager = CreateManager();

        var results = manager.SearchByIngredient("APPLE");
        Assert.Single(results);
        Assert.Equal("Recipe A", results[0].Title);
    }

    //Tests that a matching recipe is returned only once when multiple ingredients match.
    [Fact]
    public void SearchByIngredient_ShouldReturnEachRecipeOnce()
    {
        var manager = CreateManager();

        var recipe = new Recipe
        {
            Id = 30,
            Title = "Recipe C",
            Ingredients = new() { "apple", "green apples" }
        };
        manager.AddRecipe(recipe);

        var results = manager.SearchByIngredient("apple");
        var matchingRecipes = results.Count(r => r.Id == 30);
        Assert.Equal(1, matchingRecipes);
    }

    //Tests that recipes are returned in highest protein order.
    [Fact]
    public void GetHighestProteinRecipes_ShouldReturnHighestProteinRecipes()
    {
        var manager = CreateManager();
        manager.AddRecipe(new Recipe
        {
            Id = 30,
            Title = "High Protein Recipe",
            Nutrition = new NutritionInfo
            {
                ProteinG = 50
            }
        });

        manager.AddRecipe(new Recipe
        {
            Id = 40,
            Title = "Medium Protein Recipe",
            Nutrition = new NutritionInfo
            {
                ProteinG = 30
            }
        });

        manager.AddRecipe(new Recipe
        {
            Id = 50,
            Title = "Higher Protein Recipe",
            Nutrition = new NutritionInfo
            {
                ProteinG = 40
            }
        });

        var results = manager.GetHighestProteinRecipes(2);
        Assert.Equal(2, results.Count);
        Assert.Equal("High Protein Recipe", results[0].Title);
        Assert.Equal("Higher Protein Recipe", results[1].Title);
    }

    //Tests that recipes without protein data are ignored.
    [Fact]
    public void GetHighestProteinRecipes_ShouldIgnoreMissingProteinValues()
    {
        var manager = CreateManager();
        manager.AddRecipe(new Recipe
        {
            Id = 30,
            Title = "No Protein Recipe"
        });

        manager.AddRecipe(new Recipe
        {
            Id = 40,
            Title = "Protein Recipe",
            Nutrition = new NutritionInfo
            {
                ProteinG = 25
            }
        });

        var results = manager.GetHighestProteinRecipes(5);
        Assert.Single(results);
        Assert.Equal("Protein Recipe", results[0].Title);
    }

    //Tests that non-positive count returns an empty result.
    [Fact]
    public void GetHighestProteinRecipes_ShouldReturnEmptyForNonPositiveCount()
    {
        var manager = CreateManager();
        var resultsZero = manager.GetHighestProteinRecipes(0);
        var resultsNegative = manager.GetHighestProteinRecipes(-1);
        Assert.Empty(resultsZero);
        Assert.Empty(resultsNegative);
    }

    //Tests that an existing recipe can be saved and checked.
    [Fact]
    public void AddSavedRecipe_ShouldSaveExistingRecipe()
    {
        var manager = CreateManager();
        var result = manager.AddSavedRecipe(10);
        Assert.True(result);
        Assert.True(manager.IsRecipeSaved(10));
    }

    //Tests that non-existing recipes cannot be saved and duplicates are prevented.
    [Fact]
    public void AddSavedRecipe_ShouldRejectInvalidIdAndDuplicate()
    {
        var manager = CreateManager();
        var invalidResult = manager.AddSavedRecipe(999);
        var firstResult = manager.AddSavedRecipe(10);
        var duplicateResult = manager.AddSavedRecipe(10);
        Assert.False(invalidResult);
        Assert.True(firstResult);
        Assert.False(duplicateResult);
        Assert.True(manager.IsRecipeSaved(10));
    }

    //Tests that a saved recipe can be removed.
    [Fact]
    public void RemoveSavedRecipe_ShouldRemoveSavedRecipe()
    {
        var manager = CreateManager();
        manager.AddSavedRecipe(10);
        var result = manager.RemoveSavedRecipe(10);
        Assert.True(result);
        Assert.False(manager.IsRecipeSaved(10));
    }
}
