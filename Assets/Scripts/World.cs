﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Priority_Queue;

public class World : MonoBehaviour
{
	[SerializeField] private GameObject chunkPrefab;
	private static Dictionary<Int3, DataChunk> _chunks = new Dictionary<Int3, DataChunk>();
	private static Dictionary<Int2, DataColumn> _columns = new Dictionary<Int2, DataColumn>();
	private static Dictionary<Int3, DataChunk> _offloadChunks = new Dictionary<Int3, DataChunk>();
	private SimplePriorityQueue<Chunk> _loadQueue = new SimplePriorityQueue<Chunk>();
	private Queue<Chunk> _renderQueue = new Queue<Chunk>();
	private bool _rendering;

	private static int _chunkSize = 16;
	private static int _viewRange = 3;
	private static Int3 _playerPos;

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
	}

	public struct DataChunk
	{
		private readonly Int3 _pos;
		private GameObject _chunk;
		private Atlas.ID[,,] _blocks;
		private DataColumn _column;

		private bool _generated;
		private int _density;

		public DataChunk(Int3 pos, GameObject chunk)
		{
			_pos = pos;
			_chunk = chunk;
			_blocks = new Atlas.ID[_chunkSize, _chunkSize, _chunkSize];
			_column = _columns[_pos];

			_generated = false;
			_density = 0;
		}

		public void GenerateBlocks()
		{
			for (int x = 0; x < _chunkSize; ++x)
			{
				for (int y = 0; y < _chunkSize; ++y)
				{
					for (int z = 0; z < _chunkSize; ++z)
					{
						Atlas.ID block = GenerateBlock(_column, _pos.x * _chunkSize + x, _pos.y * _chunkSize + y, _pos.z * _chunkSize + z);

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
		}

		public void SetBlock(Atlas.ID block, int x, int y, int z)
		{
			// Do not give us air!
			if (block == Atlas.ID.Air) { return; }
			
			// Unnullify
			if (_blocks == null)
			{
				_blocks = new Atlas.ID[_chunkSize, _chunkSize, _chunkSize];
			}

			_blocks[x, y, z] = block;

			++_density;
		}

		public void RemoveBlock(int x, int y, int z)
		{
			// Already empty!
			if (_blocks == null || _blocks[x, y, z] == Atlas.ID.Air) { return; }

			_blocks[x, y, z] = Atlas.ID.Air;

			// Check for nullification
			if (--_density == 0)
			{
				_blocks = null;
			}
		}

		public Atlas.ID GetBlock(int x, int y, int z)
		{
			// Empty!
			if (_blocks == null)
			{
				return Atlas.ID.Air;
			}

			return _blocks[x, y, z];
		}

		public void SetChunk(GameObject chunk)
		{
			_chunk = chunk;
		}

		public GameObject GetChunk()
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

	public struct DataColumn
	{
		private readonly Int2 _pos;
		private int[,] _surface; // Start of stone layer
		private int[,] _light; // Highest opaque block

		public DataColumn(Int2 pos)
		{
			_pos = pos;
			_surface = new int[_chunkSize, _chunkSize];
			_light = new int[_chunkSize, _chunkSize];

			for (int i = 0; i < _chunkSize; ++i)
			{
				for (int j = 0; j < _chunkSize; ++j)
				{
					_surface[i, j] = GenerateTopology(i + _pos.x * _chunkSize, j + _pos.z * _chunkSize);
					//_light[i, j] = _surface[i, j] + 3;
				}
			}
		}

		public int GetSurface(int x, int z)
		{
			// Query is outside of our array
			// Assuming world —> local
			if (x < 0 || x >= _chunkSize || z < 0 || z >= _chunkSize)
			{
				x -= _pos.x * _chunkSize;
				z -= _pos.z * _chunkSize;
			}

			if (x < 0 || x >= _chunkSize || z < 0 || z >= _chunkSize)
			{
				return 0;
			}

			return _surface[x, z];
		}
	}

	void Start()
	{
		_playerPos = new Int3(Camera.main.transform.position / _chunkSize);

		GenerateChunks();

		#if UNITY_EDITOR
		UnityEditor.SceneView.FocusWindowIfItsOpen(typeof(UnityEditor.SceneView));
		#endif
	}

	void Update()
	{
		Int3 temp = new Int3(Camera.main.transform.position / _chunkSize);

        // Did player move from their chunk?
		if (CubeDistance(temp, _playerPos) > 0)
		{
			_playerPos = temp; // Set new pos
			GenerateChunks(); // Generate new chunks
			PingChunks(); // Ping old chunks for deletion
		}
	}

	private void RenderThread()
	{
		while (_loadQueue.Count > 0)
		{
			Chunk newChunkScript = _loadQueue.Dequeue();
			_renderQueue.Enqueue(newChunkScript);

			if (newChunkScript != null)
			{
				newChunkScript.GenerateBlocks();
			}
		}

		while (_renderQueue.Count > 0)
		{
			Chunk newChunkScript = _renderQueue.Dequeue();

			if (newChunkScript != null)
			{
				newChunkScript.GenerateMesh();
			}
		}

		_rendering = false;
	}

	private void GenerateChunks()
	{
		// Which direction is the player pointing in?
		Vector3 pov = Camera.main.transform.rotation * Vector3.forward;
		pov.y = 0; // Flatten it as we want it to be horizontal

		// Iterate through x, y, z
		for (int x = _playerPos.x - _viewRange - 1; x <= _playerPos.x + _viewRange + 1; ++x)
		{
			for (int z = _playerPos.z - _viewRange - 1; z <= _playerPos.z + _viewRange + 1; ++z)
			{
				Int2 grid = new Int2(x, z);

				// Does column exist?
				if (!_columns.ContainsKey(grid))
				{
					// Create new data column
					DataColumn newDataColumn = new DataColumn(grid);

					// Store in map
					_columns[grid] = newDataColumn;
				}

				for (int y = _playerPos.y - _viewRange - 1; y <= _playerPos.y + _viewRange + 1; ++y)
				{
					Int3 pos = new Int3(x, y, z);

					// Should chunk render yet?
					bool render = CubeDistance(_playerPos, pos) <= _viewRange;

                    // Does chunk exist?
					if (!_chunks.ContainsKey(pos))
					{
						// Create new chunk and get corresponding script
						GameObject newChunk = Instantiate(chunkPrefab, new Vector3(x * _chunkSize, y * _chunkSize, z * _chunkSize), Quaternion.identity);
						Chunk newChunkScript = newChunk.GetComponent<Chunk>();

						DataChunk newDataChunk;

						if (_offloadChunks.ContainsKey(pos))
						{
							// Retrieve from offload
							newDataChunk = _offloadChunks[pos];

							// Give data chunk gameobject
							newDataChunk.SetChunk(newChunk);

							// Remove from offload
							_offloadChunks.Remove(pos);
						}
						else
						{
							// Create new data chunk
							newDataChunk = new DataChunk(pos, newChunk);
						}

						// Let chunk know its corresponding data chunk and position
						newChunkScript.LoadData(pos, newDataChunk);
						newChunkScript.SetRender(render);

						// Get angle difference between vectors
						Vector3 dir = pos.Vector() * _chunkSize - Camera.main.transform.position;
						float dist = dir.magnitude;
						float diff = Vector3.Angle(pov, dir);
						float final = dist + diff;
						if (dist < _chunkSize * 2f) // Prioritize chunks immediately closest
						{
							final = dist;
						}

						// Queue chunk for generation
						_loadQueue.Enqueue(newChunkScript, final);

						// Store in map
						_chunks[pos] = newDataChunk;
					}
				}
			}
		}

		// Are there chunks that need generation?
		if (!_rendering && _loadQueue.Count > 0)
		{
			_rendering = true;
			new Thread(RenderThread).Start();
		}
	}

	private void PingChunks()
	{
		List<Int3> temp = new List<Int3>();

        // Collect all chunks that need to be deleted
		foreach (KeyValuePair<Int3, DataChunk> chunk in _chunks)
		{
			if (CubeDistance(chunk.Key, _playerPos) > _viewRange)
			{
				temp.Add(chunk.Key);
			}
		}

        // Destroy chunk
		foreach (Int3 key in temp)
		{
			DestroyChunk(key);
		}
	}

	private void DestroyChunk(Int3 pos)
	{
		Destroy(_chunks[pos].GetChunk()); // Delete corresponding gameobject
		_offloadChunks[pos] = _chunks[pos]; //Move chunk data to offload—technically should be disk or something
		_chunks.Remove(pos); // Remove chunk from main list
	}

	public static float CubeDistance(Int3 one, Int3 two)
	{
		return Mathf.Max(Mathf.Abs(one.x - two.x), Mathf.Abs(one.y - two.y), Mathf.Abs(one.z - two.z));
	}

	// This gets blocks that have already been generated in the past
	public static Atlas.ID GetBlock(Int3 pos, int x, int y, int z)
	{
		return _chunks[pos].GetBlock(x, y, z);
	}

	// This is the main world generation function per block
	public static Atlas.ID GenerateBlock(DataColumn column, int x, int y, int z)
	{
		// Topology
		float stone = column.GetSurface(x, z);
		float dirt = 3;
		
		if (y <= stone)
		{
			// Caves
			float caves = PerlinNoise(x, y * 2, z, 40, 12, 1);
			caves += PerlinNoise(x, y, z, 30, 8, 0);
			caves += PerlinNoise(x, y, z, 10, 4, 0);
			
			if (caves > 16)
			{
				return Atlas.ID.Air; // Generating caves
			}

			// Underground ores
			float coal = PerlinNoise(x, y, z, 20, 20, 0);

			if (coal > 18)
			{
				return Atlas.ID.Coal;
			}

			return Atlas.ID.Stone; // Stone layer
		}
		else if (y <= stone + dirt)
		{
			return Atlas.ID.Dirt; // Dirt cover
		}
		else if (y <= stone + dirt + 1)
		{
			return Atlas.ID.Grass; // Grass cover
		}
		else
		{
			return Atlas.ID.Air; // Open Air
		}
	}

	public static int GenerateTopology(int x, int z)
	{
		// Topology
		float stone = PerlinNoise(x, 0, z, 10, 3, 1.2f);
		stone += PerlinNoise(x, 300, z, 20, 4, 1f);
		stone += PerlinNoise(x, 500, z, 100, 20, 1f);

		// "Plateues"
		if (PerlinNoise(x, 100, z, 100, 10, 1f) >= 9f)
		{
			stone += 10;
		}

		stone += Mathf.Clamp(PerlinNoise(x, 0, z, 50, 10, 5f), 0, 10); // Craters?
		//float dirt = PerlinNoise(x, 100, z, 50, 2, 0) + 3; // At least 3 dirt
		//float dirt = 3;

		return (int) stone;
	}

	public static float PerlinNoise(float x, float y, float z, float scale, float height, float power)
	{
		float rValue;
		rValue = Noise.Noise.GetNoise(((double)x) / scale, ((double)y) / scale, ((double)z) / scale);
		rValue *= height;

		if (power != 0)
		{
			rValue = Mathf.Pow(rValue, power);
		}

		return rValue;
	}

	public static int GetChunkSize()
	{
		return _chunkSize;
	}

	public static int GetViewRange()
	{
		return _viewRange;
	}
}
