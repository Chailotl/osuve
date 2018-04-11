using System;

public struct ColumnPos : IEquatable<ColumnPos>
{
	public int x, z;

	public static ColumnPos zero = new ColumnPos(0, 0);
	public static ColumnPos north = new ColumnPos(0, 1);
	public static ColumnPos south = new ColumnPos(0, -1);
	public static ColumnPos east = new ColumnPos(1, 0);
	public static ColumnPos west = new ColumnPos(-1, 0);

	public ColumnPos(int x1, int z1)
	{
		x = x1;
		z = z1;
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

	public static bool operator ==(ColumnPos lhs, ColumnPos rhs)
	{
		return (lhs.x == rhs.x && lhs.z == rhs.z);
	}

	public static bool operator !=(ColumnPos lhs, ColumnPos rhs)
	{
		return (lhs.x != rhs.x || lhs.z != rhs.z);
	}
}
