using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DataColumn
{
	private readonly ColumnPos _pos;
	private int[,] _surface; // Start of stone layer
	//private int[,] _light; // Highest opaque block

	/// <summary>
	/// Create a new <c>DataColumn</c> with a given position.
	/// </summary>
	/// <param name="pos">Column position.</param>
	public DataColumn(ColumnPos pos)
	{
		_pos = pos;
		_surface = new int[World.chunkSize, World.chunkSize];
		//_light = new int[_chunkSize, _chunkSize];

		for (int i = 0; i < World.chunkSize; ++i)
		{
			for (int j = 0; j < World.chunkSize; ++j)
			{
				_surface[i, j] = World.GenerateTopology(i + _pos.x * World.chunkSize, j + _pos.z * World.chunkSize);
				//_light[i, j] = _surface[i, j] + 3;
			}
		}
	}

	/// <summary>
	/// Gets surface height from given position.
	/// </summary>
	/// <param name="x">X coordinate.</param>
	/// <param name="z">Z coordinate.</param>
	/// <returns>Height of terrain at given position.</returns>
	public int GetSurface(int x, int z)
	{
		// Query is outside of our array
		// Assuming world —> local
		if (x < 0 || x >= World.chunkSize || z < 0 || z >= World.chunkSize)
		{
			x -= _pos.x * World.chunkSize;
			z -= _pos.z * World.chunkSize;
		}

		if (x < 0 || x >= World.chunkSize || z < 0 || z >= World.chunkSize)
		{
			return 0;
		}

		return _surface[x, z];
	}
}