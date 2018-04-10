using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataChunk
{
	private readonly ChunkPos _pos;
	private Chunk _chunk;
	private Atlas.ID[,,] _blocks;
	private DataColumn _column;

	private bool _generated;
	private int _density;

	public DataChunk(ChunkPos pos, Chunk chunk, DataColumn column)
	{
		_pos = pos;
		_chunk = chunk;
		_blocks = new Atlas.ID[World.chunkSize, World.chunkSize, World.chunkSize];
		_column = column; //_columns[_pos];

		_generated = false;
		_density = 0;
	}

	public void GenerateBlocks()
	{
		for (int x = 0; x < World.chunkSize; ++x)
		{
			for (int y = 0; y < World.chunkSize; ++y)
			{
				for (int z = 0; z < World.chunkSize; ++z)
				{
					Atlas.ID block = World.GenerateBlock(new BlockPos(_pos, x, y, z));

					// Skip air
					if (block == Atlas.ID.Air)
					{
						continue;
					}
					_blocks[x, y, z] = block;

					++_density;
				}
			}
		}

		if (_density == 0)
		{
			_blocks = null;
		}

		_generated = true;
		_chunk.UpdateState();
	}

	public void SetBlock(Atlas.ID block, BlockPos pos)
	{
		// Do not give us air!
		if (block == Atlas.ID.Air) { return; }
		// Something is amiss, it should be our chunk pos
		if (pos.chunkPos != _pos) { return; }

		// Unnullify
		if (_blocks == null)
		{
			_blocks = new Atlas.ID[World.chunkSize, World.chunkSize, World.chunkSize];
		}

		_blocks[pos.x, pos.y, pos.z] = block;

		++_density;
	}

	public void RemoveBlock(BlockPos pos)
	{
		// Something is amiss, it should be our chunk pos
		if (pos.chunkPos != _pos) { return; }
		// Already empty!
		if (_blocks == null || _blocks[pos.x, pos.y, pos.z] == Atlas.ID.Air) { return; }

		_blocks[pos.x, pos.y, pos.z] = Atlas.ID.Air;

		// Check for nullification
		if (--_density == 0)
		{
			_blocks = null;
		}
	}

	public Atlas.ID GetBlock(BlockPos pos)
	{
		// Empty!
		if (_blocks == null)
		{
			return Atlas.ID.Air;
		}
		// Something is amiss, it should be our chunk pos
		if (pos.chunkPos != _pos) { return Atlas.ID.Air; }

		return _blocks[pos.x, pos.y, pos.z];
	}

	public void SetChunk(Chunk chunk)
	{
		_chunk = chunk;
	}

	public Chunk GetChunk()
	{
		return _chunk;
	}

	public DataColumn GetColumn()
	{
		return _column;
	}

	public bool IsGenerated()
	{
		return _generated;
	}

	public bool IsEmpty()
	{
		return (_density == 0);
	}
}