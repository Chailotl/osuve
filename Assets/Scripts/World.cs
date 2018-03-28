using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Priority_Queue;

public class World : MonoBehaviour
{
	[SerializeField] private GameObject chunkPrefab;
	private static Dictionary<Int3, DataChunk> _chunks = new Dictionary<Int3, DataChunk>();
	private static Dictionary<Int3, DataChunk> _offloadChunks = new Dictionary<Int3, DataChunk>();
	private SimplePriorityQueue<Chunk> _queue = new SimplePriorityQueue<Chunk>();
	private bool _rendering;

	private static int _chunkSize = 16;
	private static int _viewRange = 3;
	private static Int3 _playerPos;
	
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
	}

	public struct DataChunk
	{
		private readonly Int3 _pos;
		private GameObject _chunk;
		private Atlas.ID[,,] _blocks;
		private bool _generated;

		public DataChunk(Int3 pos, GameObject chunk)
		{
			_pos = pos;
			_chunk = chunk;
			_blocks = null;
			_generated = false;
		}

		public void GenerateBlocks()
		{
			for (int x = 0; x < _chunkSize; ++x)
			{
				for (int y = 0; y < _chunkSize; ++y)
				{
					for (int z = 0; z < _chunkSize; ++z)
					{
						Atlas.ID block = GenerateBlock(_pos.x * _chunkSize + x, _pos.y * _chunkSize + y, _pos.z * _chunkSize + z);

						// Skip air
						if (block == Atlas.ID.Air)
						{
							continue;
						}

						// Unnullify
						if (_blocks == null)
						{

							_blocks = new Atlas.ID[_chunkSize, _chunkSize, _chunkSize];
						}

						_blocks[x, y, z] = block;
					}
				}
			}
			_generated = true;
		}

		public void SetBlock(Atlas.ID block, int x, int y, int z)
		{
			// Unnullify
			if (_blocks == null)
			{
				
				_blocks = new Atlas.ID[_chunkSize, _chunkSize, _chunkSize];
			}

			_blocks[x, y, z] = block;

			// Do checks for nullification?
		}

		public Atlas.ID GetBlock(int x, int y, int z)
		{
			if (_blocks == null)
			{
				return Atlas.ID.Air;
			}
			else
			{
				return _blocks[x, y, z];
			}
		}

		public void SetChunk(GameObject chunk)
		{
			_chunk = chunk;
		}

		public GameObject GetChunk()
		{
			return _chunk;
		}

		public bool IsGenerated()
		{
			return _generated;
		}

		public bool IsEmpty()
		{
			return (_blocks == null);
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
		while (_queue.Count > 0)
		{
			Chunk newChunkScript = _queue.Dequeue();

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
		for (int x = _playerPos.x - _viewRange; x <= _playerPos.x + _viewRange; ++x)
		{
			for (int y = _playerPos.y - _viewRange; y <= _playerPos.y + _viewRange; ++y)
			{
				for (int z = _playerPos.z - _viewRange; z <= _playerPos.z + _viewRange; ++z)
				{
					Int3 pos = new Int3(x, y, z);

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
						_queue.Enqueue(newChunkScript, final);

						// Store in map
						_chunks[pos] = newDataChunk;
					}
				}
			}
		}

		// Are there chunks that need generation?
		if (!_rendering && _queue.Count > 0)
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
	public static Atlas.ID Block(int cx, int cy, int cz, int lx, int ly, int lz)
	{
		Int3 key = new Int3(cx, cy, cz);

		if (!_chunks.ContainsKey(key))
		{
			return GenerateBlock(cx + lx, cy + ly, cz + lz);
		}
		else
		{
			return _chunks[key].GetBlock(lx, ly, lz);
		}
	}

	// This is the main world generation function per block
	public static Atlas.ID GenerateBlock(int x, int y, int z)
	{
		if (y <= 0)
		{
			// Caves
			float caves = PerlinNoise(x, y * 2, z, 40, 12, 1);
			caves += PerlinNoise(x, y, z, 30, 8, 0);
			caves += PerlinNoise(x, y, z, 10, 4, 0);

			// Underground ores
			float ore = PerlinNoise(x, y, z, 20, 20, 0);

			if (caves > 16)
			{
				return Atlas.ID.Air; // Generating caves
			}
			else if (ore > 18)
			{
				return Atlas.ID.Ore;
			}

			return Atlas.ID.Stone; // Stone layer
		}
		else if (y > 7) // Atmosphere
		{
			return Atlas.ID.Air;
		}
		else
		{
			// Caves
			float caves = PerlinNoise(x, y * 2, z, 40, 12, 1);
			caves += PerlinNoise(x, y, z, 30, 8, 0);
			caves += PerlinNoise(x, y, z, 10, 4, 0);

			// Underground ores
			float ore = PerlinNoise(x, y, z, 20, 20, 0);

			// Topology
			float stone = PerlinNoise(x, 0, z, 10, 3, 1.2f);
			//stone += PerlinNoise(x, 300, z, 20, 4, 0) + 10; // Stone goes up to y=10
			//float dirt = PerlinNoise(x, 100, z, 50, 2, 0) + 3; // At least 3 dirt
			float dirt = 3;

			if (y <= stone)
			{
				if (caves > 16)
				{
					return Atlas.ID.Air; // Generating caves
				}
				else if (ore > 18)
				{
					return Atlas.ID.Ore;
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
