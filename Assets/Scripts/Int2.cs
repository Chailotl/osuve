using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Int2 : IEquatable<Int2>
{
	public int x, z;

	public Int2(int x1, int z1)
	{
		x = x1; z = z1;
	}

	public Int2(Vector3 vec)
	{
		x = Mathf.FloorToInt(vec.x);
		z = Mathf.FloorToInt(vec.z);
	}

	public override string ToString()
	{
		return "(" + x + ", " + z + ")";
	}

	public bool Equals(Int2 other)
	{
		return (this.x == other.x && this.z == other.z);
	}
}