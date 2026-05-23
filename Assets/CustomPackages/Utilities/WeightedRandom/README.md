# WeightedRandomList

Generic weighted random selection list that returns an item based on probability proportional to its weight.

## Example

```csharp
var lootTable = new WeightedRandomList<string>();
lootTable.Add("Common", 70f);    // ~70%
lootTable.Add("Rare", 25f);      // ~25%
lootTable.Add("Legendary", 5f);  // ~5%

// Random() — assumes non-empty, throws otherwise
string drop = lootTable.Random();
UnityEngine.Debug.Log($"Drop: {drop}");

// Update weight (returns false if not found)
lootTable.TrySetWeight("Rare", 30f);

// Remove an item
lootTable.Remove("Common");

// RandomOrDefault() — silent default
string maybe = lootTable.RandomOrDefault();
if (maybe != null) UnityEngine.Debug.Log($"Maybe: {maybe}");

// TryRandom() — explicit empty handling
if (lootTable.TryRandom(out var another))
    UnityEngine.Debug.Log($"Another: {another}");
else
    UnityEngine.Debug.LogWarning("Loot table is empty");
```