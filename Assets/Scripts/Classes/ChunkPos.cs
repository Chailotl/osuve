using System;
using UnityEngine;

public struct ChunkPos : IEquatable<ChunkPos>
{
	public int x, y, z;

	public static ChunkPos zero = new ChunkPos(0, 0, 0);
	public static ChunkPos up = new ChunkPos(0, 1, 0);
	public static ChunkPos down = new ChunkPos(0, -1, 0);
	public static ChunkPos north = new ChunkPos(0, 0, 1);
	public static ChunkPos south = new ChunkPos(0, 0, -1);
	public static ChunkPos east = new ChunkPos(1, 0, 0);
	public static ChunkPos west = new ChunkPos(-1, 0, 0);

	public ChunkPos(int x, int y, int z)
	{
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public ChunkPos(Vector3 vec)
	{
		x = Mathf.FloorToInt(vec.x);
		y = Mathf.FloorToInt(vec.y);
		z = Mathf.FloorToInt(vec.z);
	}

	// Fancy Methods

	public static float Distance(ChunkPos one, ChunkPos two)
	{
		return Mathf.Pow(Mathf.Pow(one.x - two.x, 2f) + Mathf.Pow(one.y - two.y, 2f) + Mathf.Pow(one.z - two.z, 2f), 1f / 3f);
	}

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
