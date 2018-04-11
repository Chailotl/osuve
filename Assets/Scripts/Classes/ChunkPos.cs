using System;
using UnityEngine;

public struct ChunkPos : IEquatable<ChunkPos>
{
	/// <summary>Coordinate.</summary>
	public int x, y, z;

	/// <summary>Shorthand for writing <c>ChunkPos(0, 0, 0)</c>.</summary>
	public readonly static ChunkPos zero = new ChunkPos(0, 0, 0);
	/// <summary>Shorthand for writing <c>ChunkPos(0, 1, 0)</c>.</summary>
	public readonly static ChunkPos up = new ChunkPos(0, 1, 0);
	/// <summary>Shorthand for writing <c>ChunkPos(0, -1, 0)</c>.</summary>
	public readonly static ChunkPos down = new ChunkPos(0, -1, 0);
	/// <summary>Shorthand for writing <c>ChunkPos(0, 0, 1)</c>.</summary>
	public readonly static ChunkPos north = new ChunkPos(0, 0, 1);
	/// <summary>Shorthand for writing <c>ChunkPos(0, 0, -1)</c>.</summary>
	public readonly static ChunkPos south = new ChunkPos(0, 0, -1);
	/// <summary>Shorthand for writing <c>ChunkPos(1, 0, 0)</c>.</summary>
	public readonly static ChunkPos east = new ChunkPos(1, 0, 0);
	/// <summary>Shorthand for writing <c>ChunkPos(-1, 0, 0)</c>.</summary>
	public readonly static ChunkPos west = new ChunkPos(-1, 0, 0);

	/// <summary>
	/// Create a new <c>ChunkPos</c> using explicit <paramref name="x"/>, <paramref name="y"/>, and <paramref name="z"/> coordinates.
	/// </summary>
	/// <param name="x">X coordinate.</param>
	/// <param name="y">Y coordinate.</param>
	/// <param name="z">Z coordinate.</param>
	/// See <see cref="ChunkPos.ChunkPos(Vector3)"/> for taking Vector3.
	public ChunkPos(int x, int y, int z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	/// <summary>
	/// Create a new <c>ChunkPos</c> using a Vector3.
	/// </summary>
	/// <param name="vec">A Vector3.</param>
	/// See <see cref="ChunkPos.ChunkPos(int, int, int)"/> for taking separate ints.
	public ChunkPos(Vector3 vec)
	{
		x = Mathf.FloorToInt(vec.x);
		y = Mathf.FloorToInt(vec.y);
		z = Mathf.FloorToInt(vec.z);
	}
	
	/// <summary>
	/// Returns the distance between two ChunkPos.
	/// </summary>
	/// <param name="one">First ChunkPos.</param>
	/// <param name="two">Second ChunkPos.</param>
	/// <returns>Distance between two ChunkPos.</returns>
	/// See <see cref="ChunkPos.CubeDistance(ChunkPos, ChunkPos)"/> for cubic distances.
	public static float Distance(ChunkPos one, ChunkPos two)
	{
		return Mathf.Pow(Mathf.Pow(one.x - two.x, 2f) + Mathf.Pow(one.y - two.y, 2f) + Mathf.Pow(one.z - two.z, 2f), 1f / 3f);
	}

	/// <summary>
	/// Returns the cubic distance between two ChunkPos.
	/// </summary>
	/// <param name="one">First ChunkPos.</param>
	/// <param name="two">Second ChunkPos.</param>
	/// <returns>Cubic distance between two ChunkPos.</returns>
	/// See <see cref="ChunkPos.Distance(ChunkPos, ChunkPos)"/> for non-cubic distances.
	public static float CubeDistance(ChunkPos one, ChunkPos two)
	{
		return Mathf.Max(Mathf.Abs(one.x - two.x), Mathf.Abs(one.y - two.y), Mathf.Abs(one.z - two.z));
	}

	// Base methods

	public override string ToString()
	{
		return "Chunk: (" + x + ", " + y + ", " + z + ")";
	}

	public bool Equals(ChunkPos other)
	{
		return (this.x == other.x && this.y == other.y && this.z == other.z);
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		var hashCode = 373119288;
		hashCode = hashCode * -1521134295 + base.GetHashCode();
		hashCode = hashCode * -1521134295 + x.GetHashCode();
		hashCode = hashCode * -1521134295 + y.GetHashCode();
		hashCode = hashCode * -1521134295 + z.GetHashCode();
		return hashCode;
	}
	
	// Operators

	public static ChunkPos operator +(ChunkPos lhs, ChunkPos rhs)
	{
		return new ChunkPos(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
	}

	public static ChunkPos operator -(ChunkPos lhs, ChunkPos rhs)
	{
		return new ChunkPos(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
	}

	public static Vector3 operator *(ChunkPos lhs, float rhs)
	{
		return new Vector3(lhs.x * rhs, lhs.y * rhs, lhs.z * rhs);
	}

	public static bool operator ==(ChunkPos lhs, ChunkPos rhs)
	{
		return (lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z);
	}

	public static bool operator !=(ChunkPos lhs, ChunkPos rhs)
	{
		return (lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z);
	}

	public static implicit operator ColumnPos(ChunkPos rhs)
	{
		return new ColumnPos(rhs.x, rhs.z);
	}
}
