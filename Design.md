# Design

Complete this document for **Part B**.

## 1. Saved recipe collection

I selected HashSet<int> for the saved recipe feature.
The HashSet stores the IDs of saved recipes. A HashSet does not allow duplicate values, so the same recipe cannot be saved twice. It also supports add, remove and membership checks efficiently. This makes it suitable for a saved or favourite recipe feature.

## 2. Integration

The Part B features are added to the existing RecipeManager and use the existing recipe catalogue. The title search, ingredient search and protein report use LINQ with the recipes in the catalogue. The saved recipe feature uses a HashSet<int> to store recipe IDs. The integration tests search for a recipe, save the recipe, check that it is saved, remove the recipe from the saved collection, and check that it is no longer saved.

## 3. Basic complexity

| Operation | Structure | Expected complexity | Reason |
| --- | --- | --- | --- |
| Lookup recipe by ID | Dictionary | Average O(1) | Hash-based key lookup. |
| Traverse cooking plan | LinkedList | O(n) | Each planned recipe may need to be visited. |
| Complete next instruction | Queue | O(1) | The item at the front is removed. |
| LINQ title/ingredient search | Recipe collection | O(n) | Each recipe may need to be inspected. |

| LINQ protein report | Recipe collection | O(n log n) | Recipes are filtered and then sorted by protein value. |
| Check whether a recipe is saved | HashSet | Average O(1) | Hash-based membership check. |