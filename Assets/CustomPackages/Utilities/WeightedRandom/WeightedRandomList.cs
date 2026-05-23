using System;
using System.Collections.Generic;
using UnityEngine;

namespace ThanhDV.Utilities
{
    [Serializable]
    public class WeightedRandomList<T> : ISerializationCallbackReceiver
    {
        [Serializable]
        public struct Pair
        {
            public T Item;

            [Min(0f)] public float Weight;

            public Pair(T item, float weight)
            {
                Item = item;
                Weight = weight;
            }
        }

        [SerializeField]
        private List<Pair> _pairs = new();

        /// <summary>
        /// Number of entries in the list.
        /// </summary>
        public int Count => _pairs.Count;

        /// <summary>
        /// Read-only view of all item-weight pairs.
        /// </summary>
        public IReadOnlyList<Pair> Pairs => _pairs;

        /// <summary>
        /// Sum of all weights in the list.
        /// </summary>
        public float TotalWeight
        {
            get
            {
                float sum = 0f;
                foreach (var p in _pairs)
                    sum += p.Weight;
                return sum;
            }
        }


        /// <summary>
        /// Adds an item with a weight.
        /// </summary>
        /// <param name="item">Item to add.</param>
        /// <param name="weight">Pick weight. Must be finite and non-negative.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="weight"/> is negative, <c>NaN</c>, or infinity.
        /// </exception>
        public void Add(T item, float weight)
        {
            ValidateWeight(weight, nameof(weight));
            _pairs.Add(new Pair(item, weight));
        }

        /// <summary>
        /// Adds an item, or updates the first matching item's weight.
        /// </summary>
        /// <param name="item">Item to add or update.</param>
        /// <param name="weight">New weight. Must be finite and non-negative.</param>
        /// <returns><c>true</c> if added; <c>false</c> if updated.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="weight"/> is negative, <c>NaN</c>, or infinity.
        /// </exception>
        public bool AddOrUpdate(T item, float weight)
        {
            ValidateWeight(weight, nameof(weight));
            for (int i = 0; i < _pairs.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_pairs[i].Item, item))
                {
                    _pairs[i] = new Pair(_pairs[i].Item, weight);
                    return false;
                }
            }
            _pairs.Add(new Pair(item, weight));
            return true;
        }

        /// <summary>
        /// Removes the first matching item.
        /// </summary>
        /// <param name="item">Item to remove.</param>
        /// <returns><c>true</c> if an item was removed.</returns>
        public bool Remove(T item)
        {
            for (int i = 0; i < _pairs.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_pairs[i].Item, item))
                {
                    _pairs.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes all items that match a predicate.
        /// </summary>
        /// <param name="match">Predicate used to choose items to remove.</param>
        /// <returns>Number of removed items.</returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="match"/> is <c>null</c>.
        /// </exception>
        public int RemoveAll(Predicate<T> match)
        {
            if (match == null) throw new ArgumentNullException(nameof(match));
            return _pairs.RemoveAll(p => match(p.Item));
        }

        /// <summary>
        /// Removes every item.
        /// </summary>
        public void Clear() => _pairs.Clear();

        /// <summary>
        /// Sets the weight of the first matching item.
        /// </summary>
        /// <param name="item">Item to update.</param>
        /// <param name="newWeight">New weight. Must be finite and non-negative.</param>
        /// <returns><c>true</c> if the item was found and updated.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="newWeight"/> is negative, <c>NaN</c>, or infinity.
        /// </exception>
        public bool TrySetWeight(T item, float newWeight)
        {
            ValidateWeight(newWeight, nameof(newWeight));
            for (int i = 0; i < _pairs.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_pairs[i].Item, item))
                {
                    _pairs[i] = new Pair(_pairs[i].Item, newWeight);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks whether the list contains an item.
        /// </summary>
        /// <param name="item">Item to find.</param>
        /// <returns><c>true</c> if a matching item exists.</returns>
        public bool Contains(T item)
        {
            foreach (var p in _pairs)
            {
                if (EqualityComparer<T>.Default.Equals(p.Item, item))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the weight of the first matching item.
        /// </summary>
        /// <param name="item">Item to find.</param>
        /// <param name="weight">Weight of the found item, or <c>0</c> if not found.</param>
        /// <returns><c>true</c> if the item was found; otherwise <c>false</c>.</returns>
        public bool TryGetWeight(T item, out float weight)
        {
            foreach (var p in _pairs)
            {
                if (EqualityComparer<T>.Default.Equals(p.Item, item))
                {
                    weight = p.Weight;
                    return true;
                }
            }
            weight = 0f;
            return false;
        }

        private static void ValidateWeight(float weight, string paramName)
        {
            if (weight < 0f || float.IsNaN(weight) || float.IsInfinity(weight))
            {
                throw new ArgumentOutOfRangeException(paramName, weight, "Weight must be a finite non-negative number (zero is allowed).");
            }
        }

        /// <summary>
        /// Picks a random item using its weight.
        /// </summary>
        /// <returns>The selected item.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the list is empty or all weights are zero.
        /// </exception>
        public T Random()
        {
            if (!TryRandom(out var item))
            {
                if (_pairs.Count == 0)
                    throw new InvalidOperationException("Cannot pick from an empty WeightedRandomList<T>. Use RandomOrDefault() or TryRandom(out T) to handle empty lists.");
                throw new InvalidOperationException("Cannot pick from a WeightedRandomList<T> where all items have weight 0. Set at least one positive weight, or use RandomOrDefault()/TryRandom(out T).");
            }
            return item;
        }

        /// <summary>
        /// Picks a random item, or returns <c>default(T)</c> when none can be picked.
        /// </summary>
        public T RandomOrDefault()
        {
            TryRandom(out var item);
            return item;
        }

        /// <summary>
        /// Tries to pick a random item using its weight.
        /// </summary>
        /// <param name="item">Selected item, or <c>default(T)</c> if none can be picked.</param>
        /// <returns><c>true</c> if an item was picked.</returns>
        public bool TryRandom(out T item)
        {
            if (_pairs.Count == 0)
            {
                item = default;
                return false;
            }

            float totalWeight = 0;
            foreach (Pair p in _pairs)
            {
                totalWeight += p.Weight;
            }

            // All weights are zero, so no item is pickable.
            if (totalWeight <= 0f)
            {
                item = default;
                return false;
            }

            float value = UnityEngine.Random.value * totalWeight;
            float sumWeight = 0;

            foreach (Pair p in _pairs)
            {
                sumWeight += p.Weight;
                if (sumWeight >= value)
                {
                    item = p.Item;
                    return true;
                }
            }

            // Safe fallback for floating-point drift: list is guaranteed non-empty here.
            item = _pairs[_pairs.Count - 1].Item;
            return true;
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize() { }

        /// <summary>
        /// Clamps invalid serialized weights to zero after deserialization.
        /// </summary>
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (_pairs == null) return;

            bool fixedAny = false;
            for (int i = 0; i < _pairs.Count; i++)
            {
                var p = _pairs[i];
                if (p.Weight < 0f || float.IsNaN(p.Weight) || float.IsInfinity(p.Weight))
                {
                    _pairs[i] = new Pair(p.Item, 0f);
                    fixedAny = true;
                }
            }

            if (fixedAny)
            {
                Debug.LogWarning($"[WeightedRandomList<{typeof(T).Name}>] One or more entries had an invalid weight (negative/NaN/Infinity). Clamped to 0.");
            }
        }
    }
}
