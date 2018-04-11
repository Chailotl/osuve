using System;

public struct ColumnPos : IEquatable<ColumnPos>
{
	/// <summary>Coordinate.</summary>
	public int x, z;

	/// <summary>Shorthand for writing <c>ColumnPos(0, 0)</c>.</summary>
	public readonly static ColumnPos zero = new ColumnPos(0, 0);
	/// <summary>Shorthand for writing <c>ColumnPos(0, 1)</c>.</summary>
	public readonly static ColumnPos north = new ColumnPos(0, 1);
	/// <summary>Shorthand for writing <c>ColumnPos(0, -1)</c>.</summary>
	public readonly static ColumnPos south = new ColumnPos(0, -1);
	/// <summary>Shorthand for writing <c>ColumnPos(1, 0)</c>.</summary>
	public readonly static ColumnPos east = new ColumnPos(1, 0);
	/// <summary>Shorthand for writing <c>ColumnPos(-1, 0)</c>.</summary>
	public readonly static ColumnPos west = new ColumnPos(-1, 0);

	/// <summary>
	/// Create a new <c>ColumnPos</c> using explicit <paramref name="x"/> and <paramref name="z"/> coordinates.
	/// </summary>
	/// <remarks>ColumnPos are placed on an x-z plane and represent the corresponding y axis, hence why y is not used</remarks>
	/// <param name="x">X coordinate.</param>
	/// <param name="z">Z coordinate.</param>
	public ColumnPos(int x, int z)
	{
		this.x = x;
		this.z = z;
	}

	// Base methods
	
	public override string ToString()
	{
		return "Column: (" + x + ", " + z + ")";
	}

	public bool Equals(ColumnPos other)
	{
		return (this.x == other.x && this.z == other.z);
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
		hashCode = hashCode * -1521134295 + z.GetHashCode();
		return hashCode;
	}

	// Operators

	public static ColumnPos operator +(ColumnPos lhs, ColumnPos rhs)
	{
		return new ColumnPos(lhs.x + rhs.x, lhs.z + rhs.z);
	}

	public static ColumnPos operator -(ColumnPos lhs, ColumnPos rhs)
	{
		return new ColumnPos(lhs.x - rhs.x, lhs.z - rhs.z);
	}

	public static bool operator ==(ColumnPos lhs, ColumnPos rhs)
	{
		return (lhs.x == rhs.x && lhs.z == rhs.z);
	}

	public static bool operator !=(ColumnPos lhs, ColumnPos rhs)
	{
		return (lhs.x != rhs.x || lhs.z != rhs.z);
	}
}
