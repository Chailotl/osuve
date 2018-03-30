using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Int3 : IEquatable<Int3>
{
	public int x, y, z;

	public Int3(int x1, int y1, int z1)
	{
		x = x1; y = y1; z = z1;
	}

	public Int3(Vector3 vec)
	{
		x = Mathf.FloorToInt(vec.x);
		y = Mathf.FloorToInt(vec.y);
		z = Mathf.FloorToInt(vec.z);
	}

	public override string ToString()
	{
		return "(" + x + ", " + y + ", " + z + ")";
	}

	public bool Equals(Int3 other)
	{
		return (this.x == other.x && this.y == other.y && this.z == other.z);
	}

	public Vector3 Vector()
	{
		return new Vector3(this.x, this.y, this.z);
	}

	public static implicit operator Int2(Int3 str)
	{
		return new Int2(str.x, str.z);
	}

	public Int3 Add(Int3 other)
	{
		return new Int3(x + other.x, y + other.y, z + other.z);
	}
}