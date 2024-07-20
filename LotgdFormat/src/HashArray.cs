// SPDX-License-Identifier: GPL-2.0-only
using System.Runtime.CompilerServices;

namespace LotgdFormat;

/// <summary>
/// Custom hash table using a flat array and a perfect hash algorithm. <br/>
/// The hash function is just (key + magicNumber).GetHashCode() & (bucketSize). <br/>
/// The offset is determined at construction. <br/>
/// The bucketsize is the nearest power of two (minus 1) to the input size, but at minimum 127.
/// </summary>
/// <typeparam name="T">
/// Datatype for the contained data.
/// </typeparam>
internal class HashArray<T> where T : class {
	private int _hashIndex;
	private T[] _bucket;

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	internal T? Get(in char key) {
		var hash = this.Hash(key);
		if (hash > _bucket.Length) {
			return null;
		}
		return _bucket[hash];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int GetSize(uint size) {
		size--;
		size |= size >> 1;
		size |= size >> 2;
		size |= size >> 4;
		size |= size >> 8;
		size |= size >> 16;
		size++;
		return (int)(size << 1) - 1;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int FindHashIndex(ReadOnlySpan<char> array, int bucketSize) {
		Span<bool> used = stackalloc bool[bucketSize];
		for (ushort index = 0; index < ushort.MaxValue;) {
			for (int i = 0; i < array.Length; i++) {
				var x = (array[i] + index).GetHashCode() & (bucketSize);
				if (x > used.Length || used[x] == true) {
					goto outer;
				} else {
					used[x] = true;
				}
			}
			return index;
		outer:
			used.Clear();
			index++;
		}
		throw new Exception("Could not find suitable hash offset");
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private int Hash(in char input) {
		return (input + this._hashIndex).GetHashCode() & (this._bucket.Length);
	}

	internal HashArray(ReadOnlySpan<char> keys, Span<T> data) {
		int bucketSize = Math.Max(127, GetSize((uint)data.Length));
		this._hashIndex = FindHashIndex(keys, bucketSize);
		this._bucket = new T[bucketSize];
		for (int i = 0; i < data.Length; i++) {
			this._bucket[this.Hash(keys[i])] = data[i];
		}
	}
}
