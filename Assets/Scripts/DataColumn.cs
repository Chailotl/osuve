using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DataColumn
{
	private readonly Int2 _pos;
	private int[,] _surface; // Start of stone layer
	//private int[,] _light; // Highest opaque block

	public DataColumn(Int2 pos)
	{
		_pos = pos;
		_surface = new int[World.GetChunkSize(), World.GetChunkSize()];
		//_light = new int[_chunkSize, _chunkSize];

		for (int i = 0; i < World.GetChunkSize(); ++i)
		{
			for (int j = 0; j < World.GetChunkSize(); ++j)
			{
				_surface[i, j] = World.GenerateTopology(i + _pos.x * World.GetChunkSize(), j + _pos.z * World.GetChunkSize());
				//_light[i, j] = _surface[i, j] + 3;
			}
		}
	}

	public int GetSurface(int x, int z)
	{
		// Query is outside of our array
		// Assuming world —> local
		if (x < 0 || x >= World.GetChunkSize() || z < 0 || z >= World.GetChunkSize())
		{
			x -= _pos.x * World.GetChunkSize();
			z -= _pos.z * World.GetChunkSize();
		}

		if (x < 0 || x >= World.GetChunkSize() || z < 0 || z >= World.GetChunkSize())
		{
			return 0;
		}

		return _surface[x, z];
	}
}