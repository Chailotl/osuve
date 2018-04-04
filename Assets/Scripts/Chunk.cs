using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Chunk : MonoBehaviour
{
	// Mesh generation
	private List<Vector3> _newVerts = new List<Vector3>();
	private List<int> _newTris = new List<int>();
	private List<Vector2> _newUV = new List<Vector2>();
	private List<Color> _newColors = new List<Color>();
	private int _faceCount;

	private Mesh _mesh;
	private MeshCollider _col;
	private bool _updateMesh;
	private bool _clearMesh;

	/*
	 * Fresh - The chunk has been freshly created, but it has no data associated
	 * Prepped - The chunk now has its basic data
	 * Generating - The chunk is actively generating or retrieving block data
	 * Loaded - The chunk has its block data loaded
	 * Rendered - The chunk is actively rendering
	 */
	public enum State { Fresh, Prepped, Generating, Loaded, Rendered };

	private State _state = State.Fresh;

	//private Dictionary<Int3, DataChunk>;
	private DataChunk _upChunk;
	private DataChunk _downChunk;
	private DataChunk _northChunk;
	private DataChunk _southChunk;
	private DataChunk _eastChunk;
	private DataChunk _westChunk;

	// Informatics
	private int _chunkSize;
	private Int3 _chunkPos;
	private DataChunk _chunkData;
	
	// Debug
	public bool isolateMesh;
	private bool _updateIso;

	public void LoadData(Int3 pos, DataChunk chunkData)
	{
		// One-time only!
		if (_state == State.Fresh)
		{
			_state = State.Prepped;

			_chunkSize = World.GetChunkSize();
			_chunkPos = pos;
			_chunkData = chunkData;
		}
	}

	// We don't want others changing the state,
	// so we'll ping the chunk to update it for itself
	public void UpdateState()
	{
		switch (_state)
		{
			case State.Generating:
				if (_chunkData.IsGenerated())
				{
					_state = State.Loaded;

					// Attempt to get cardinal chunks
					// This isn't guaranteed to pass, but as these chunks
					// generate, they'll inform us of their existence
					_upChunk = World.GetChunk(_chunkPos.Add(new Int3(0, 1, 0)));
					if (_upChunk != null && !_upChunk.IsGenerated()) { _upChunk = null; }
					_downChunk = World.GetChunk(_chunkPos.Add(new Int3(0, -1, 0)));
					if (_downChunk != null && !_downChunk.IsGenerated()) { _downChunk = null; }
					_northChunk = World.GetChunk(_chunkPos.Add(new Int3(0, 0, 1)));
					if (_northChunk != null && !_northChunk.IsGenerated()) { _northChunk = null; }
					_southChunk = World.GetChunk(_chunkPos.Add(new Int3(0, 0, -1)));
					if (_southChunk != null && !_southChunk.IsGenerated()) { _southChunk = null; }
					_eastChunk = World.GetChunk(_chunkPos.Add(new Int3(1, 0, 0)));
					if (_eastChunk != null && !_eastChunk.IsGenerated()) { _eastChunk = null; }
					_westChunk = World.GetChunk(_chunkPos.Add(new Int3(-1, 0, 0)));
					if (_westChunk != null && !_westChunk.IsGenerated()) { _westChunk = null; }

					// Let's now ping them like the baka we are
					if (_upChunk != null) { _upChunk.GetChunk().Ping(_chunkData, Atlas.Dir.Down); }
					if (_downChunk != null) { _downChunk.GetChunk().Ping(_chunkData, Atlas.Dir.Up); }
					if (_northChunk != null) { _northChunk.GetChunk().Ping(_chunkData, Atlas.Dir.South); }
					if (_southChunk != null) { _southChunk.GetChunk().Ping(_chunkData, Atlas.Dir.North); }
					if (_eastChunk != null) { _eastChunk.GetChunk().Ping(_chunkData, Atlas.Dir.West); }
					if (_westChunk != null) { _westChunk.GetChunk().Ping(_chunkData, Atlas.Dir.East); }

					GenerateMesh();
				}
				break;
		}
	}

	// A chunk exists!
	public void Ping(DataChunk chunk, Atlas.Dir dir)
	{

		bool nullCheck = false;

		switch (dir)
		{
			case Atlas.Dir.Up:
				nullCheck = (_upChunk == null);
				_upChunk = chunk;
				break;

			case Atlas.Dir.Down:
				nullCheck = (_downChunk == null);
				_downChunk = chunk;
				break;

			case Atlas.Dir.North:
				nullCheck = (_northChunk == null);
				_northChunk = chunk;
				break;

			case Atlas.Dir.South:
				nullCheck = (_southChunk == null);
				_southChunk = chunk;
				break;

			case Atlas.Dir.East:
				nullCheck = (_eastChunk == null);
				_eastChunk = chunk;
				break;

			case Atlas.Dir.West:
				nullCheck = (_westChunk == null);
				_westChunk = chunk;
				break;
		}

		// We shouldn't run if we already had the chunk before
		if (nullCheck && _state == State.Rendered)
		{
			// Get new surfaces!
			GenerateMesh(dir);
		}
	}

	void Start()
	{
		_mesh = GetComponent<MeshFilter>().mesh;
		_col = GetComponent<MeshCollider>();
	}

	void Update()
	{
		// Should my mesh be updated?
		if (_updateMesh)
		{
			_updateMesh = false;
			UpdateMesh();
		}

		if (isolateMesh != _updateIso)
		{
			_updateIso = isolateMesh;
			GenerateMesh();
		}
	}

	public void GenerateBlocks()
	{
		// Check if data chunk blocks are generated
		if (!_chunkData.IsGenerated())
		{
			_state = State.Generating;
			_chunkData.GenerateBlocks();
		}
	}

	public void GenerateMesh()
	{
		// Iterate through x, y, z
		for (int x = 0; x < _chunkSize; x++)
		{
			for (int y = 0; y < _chunkSize; y++)
			{
				for (int z = 0; z < _chunkSize; z++)
				{
					Atlas.ID block = Block(x, y, z);

					// Generate the mesh and texturize
					if (block != Atlas.ID.Air)
					{
						if (Block(x, y + 1, z) == Atlas.ID.Air)
						{
							CubeUp(x, y, z, block);
						}

						if (Block(x, y - 1, z) == Atlas.ID.Air)
						{
							CubeDown(x, y, z, block);
						}

						if (Block(x + 1, y, z) == Atlas.ID.Air)
						{
							CubeEast(x, y, z, block);
						}

						if (Block(x - 1, y, z) == Atlas.ID.Air)
						{
							CubeWest(x, y, z, block);
						}

						if (Block(x, y, z + 1) == Atlas.ID.Air)
						{
							CubeNorth(x, y, z, block);
						}

						if (Block(x, y, z - 1) == Atlas.ID.Air)
						{
							CubeSouth(x, y, z, block);
						}
					}
				}
			}
		}

		_clearMesh = true;
		_updateMesh = true;
	}

	// This generates only border mesh stuff
	public void GenerateMesh(Atlas.Dir dir)
	{
		// Start, end
		int xS = 0; int xE = _chunkSize;
		int yS = 0; int yE = _chunkSize;
		int zS = 0; int zE = _chunkSize;
		
		switch (dir)
		{
			case Atlas.Dir.Up:
				yS = _chunkSize; ++yE;
				break;

			case Atlas.Dir.Down:
				yE = 1;
				break;

			case Atlas.Dir.East:
				xS = _chunkSize; ++xE;
				break;

			case Atlas.Dir.West:
				xE = 1;
				break;

			case Atlas.Dir.North:
				zS = _chunkSize; ++zE;
				break;

			case Atlas.Dir.South:
				zE = 1;
				break;
		}

		// Iterate through x, y, z
		for (int x = xS; x < xE; x++)
		{
			for (int y = yS; y < yE; y++)
			{
				for (int z = zS; z < zE; z++)
				{
					Atlas.ID block = Block(x, y, z);

					// Generate the mesh and texturize
					if (block != Atlas.ID.Air)
					{
						// Out of bounds are done in a separate method
						if (dir == Atlas.Dir.Up && Block(x, y + 1, z) == Atlas.ID.Air)
						{
							CubeUp(x, y, z, block);
						}

						if (dir == Atlas.Dir.Down && Block(x, y - 1, z) == Atlas.ID.Air)
						{
							CubeDown(x, y, z, block);
						}

						if (dir == Atlas.Dir.East && Block(x + 1, y, z) == Atlas.ID.Air)
						{
							CubeEast(x, y, z, block);
						}

						if (dir == Atlas.Dir.West && Block(x - 1, y, z) == Atlas.ID.Air)
						{
							CubeWest(x, y, z, block);
						}

						if (dir == Atlas.Dir.North && Block(x, y, z + 1) == Atlas.ID.Air)
						{
							CubeNorth(x, y, z, block);
						}

						if (dir == Atlas.Dir.South && Block(x, y, z - 1) == Atlas.ID.Air)
						{
							CubeSouth(x, y, z, block);
						}
					}
				}
			}
		}

		_updateMesh = true;
	}

	// Local block to world blocks
	private Atlas.ID Block(int x, int y, int z)
	{
		if (x >= 0 && x < _chunkSize && y >= 0 && y < _chunkSize && z >= 0 && z < _chunkSize)
		{
			// In bounds, we have the data available to us
			return _chunkData.GetBlock(x, y, z);
		}
		else if (isolateMesh)
		{
			return Atlas.ID.Air;
		}
		else
		{
			// Outside of bounds, need to fetch
			if (x == -1 && _westChunk != null)
			{
				x = _chunkSize - 1;
				return _westChunk.GetBlock(x, y, z); // Error
			}
			else if (x == _chunkSize && _eastChunk != null)
			{
				x = 0;
				return _eastChunk.GetBlock(x, y, z); // Error
			}
			else if (y == -1 && _downChunk != null)
			{
				y = _chunkSize - 1;
				return _downChunk.GetBlock(x, y, z); // Error
			}
			else if (y == _chunkSize && _upChunk != null)
			{
				y = 0;
				return _upChunk.GetBlock(x, y, z);
			}
			else if (z == -1 && _southChunk != null)
			{
				z = _chunkSize - 1;
				return _southChunk.GetBlock(x, y, z); // Error
			}
			else if (z == _chunkSize && _northChunk != null)
			{
				z = 0;
				return _northChunk.GetBlock(x, y, z); // Error
			}

			return Atlas.ID.Solid; // Don't generate a mesh
		}
	}

	private void UpdateMesh()
	{
		if (_clearMesh)
		{
			_clearMesh = false;
			_mesh.Clear();
		}
		_mesh.vertices = _newVerts.ToArray();
		_mesh.uv = _newUV.ToArray();
		_mesh.triangles = _newTris.ToArray();
		_mesh.colors = _newColors.ToArray();
		_mesh.RecalculateNormals();

		_col.sharedMesh = null;
		_col.sharedMesh = _mesh;

		//_newVerts.Clear();
		//_newUV.Clear();
		//_newTris.Clear();
		//_newColors.Clear();

		_faceCount = 0;

		// We are done rendering!...for now!
		_state = State.Rendered;
	}

	private void CubeUp(int x, int y, int z, Atlas.ID block)
	{
		_newVerts.Add(new Vector3(x, y, z + 1));
		_newVerts.Add(new Vector3(x + 1, y, z + 1));
		_newVerts.Add(new Vector3(x + 1, y, z));
		_newVerts.Add(new Vector3(x, y, z));

		Atlas.Dir dir = Atlas.Dir.Up;
		Color color = Color.white;

		if (block == Atlas.ID.Grass)
		{
			color = Atlas.Colors["Normal_1"] * 2f; // Multiplier that most Unity shaders seem to use to brighten
		}

		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);

		Vector2 texturePos = Atlas.GetTexture(block, dir);

		Cube(texturePos);
	}

	private void CubeDown(int x, int y, int z, Atlas.ID block)
	{
		_newVerts.Add(new Vector3(x, y - 1, z));
		_newVerts.Add(new Vector3(x + 1, y - 1, z));
		_newVerts.Add(new Vector3(x + 1, y - 1, z + 1));
		_newVerts.Add(new Vector3(x, y - 1, z + 1));

		Atlas.Dir dir = Atlas.Dir.Down;
		Color color = Color.white;

		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);

		Vector2 texturePos = Atlas.GetTexture(block, dir);

		Cube(texturePos);
	}

	private void CubeNorth(int x, int y, int z, Atlas.ID block)
	{
		_newVerts.Add(new Vector3(x + 1, y - 1, z + 1));
		_newVerts.Add(new Vector3(x + 1, y, z + 1));
		_newVerts.Add(new Vector3(x, y, z + 1));
		_newVerts.Add(new Vector3(x, y - 1, z + 1));

		Atlas.Dir dir = Atlas.Dir.North;
		Color color = Color.white;

		if (false && block == Atlas.ID.Grass && Block(x, y - 1, z + 1) == Atlas.ID.Grass)
		{
			dir = Atlas.Dir.Up;
			color = Atlas.Colors["Normal_1"] * 2f; // Multiplier that most Unity shaders seem to use to brighten
		}

		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);

		Vector2 texturePos = Atlas.GetTexture(block, dir);

		Cube(texturePos);
	}

	private void CubeSouth(int x, int y, int z, Atlas.ID block)
	{
		_newVerts.Add(new Vector3(x, y - 1, z));
		_newVerts.Add(new Vector3(x, y, z));
		_newVerts.Add(new Vector3(x + 1, y, z));
		_newVerts.Add(new Vector3(x + 1, y - 1, z));

		Atlas.Dir dir = Atlas.Dir.South;
		Color color = Color.white;

		if (false && block == Atlas.ID.Grass && Block(x, y - 1, z - 1) == Atlas.ID.Grass)
		{
			dir = Atlas.Dir.Up;
			color = Atlas.Colors["Normal_1"] * 2f; // Multiplier that most Unity shaders seem to use to brighten
		}

		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);

		Vector2 texturePos = Atlas.GetTexture(block, dir);

		Cube(texturePos);
	}

	private void CubeEast(int x, int y, int z, Atlas.ID block)
	{
		_newVerts.Add(new Vector3(x + 1, y - 1, z));
		_newVerts.Add(new Vector3(x + 1, y, z));
		_newVerts.Add(new Vector3(x + 1, y, z + 1));
		_newVerts.Add(new Vector3(x + 1, y - 1, z + 1));

		Atlas.Dir dir = Atlas.Dir.East;
		Color color = Color.white;

		if (false && block == Atlas.ID.Grass && Block(x + 1, y - 1, z) == Atlas.ID.Grass)
		{
			dir = Atlas.Dir.Up;
			color = Atlas.Colors["Normal_1"] * 2f; // Multiplier that most Unity shaders seem to use to brighten
		}

		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);

		Vector2 texturePos = Atlas.GetTexture(block, dir);

		Cube(texturePos);
	}

	private void CubeWest(int x, int y, int z, Atlas.ID block)
	{
		_newVerts.Add(new Vector3(x, y - 1, z + 1));
		_newVerts.Add(new Vector3(x, y, z + 1));
		_newVerts.Add(new Vector3(x, y, z));
		_newVerts.Add(new Vector3(x, y - 1, z));

		Atlas.Dir dir = Atlas.Dir.West;
		Color color = Color.white;
		
		if (false && block == Atlas.ID.Grass && Block(x - 1, y - 1, z) == Atlas.ID.Grass)
		{
			dir = Atlas.Dir.Up;
			color = Atlas.Colors["Normal_1"] * 2f; // Multiplier that most Unity shaders seem to use to brighten
		}

		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);
		_newColors.Add(color);

		Vector2 texturePos = Atlas.GetTexture(block, dir);

		Cube(texturePos);
	}

	private void Cube(Vector2 texturePos)
	{
		_newTris.Add(_faceCount * 4); //1
		_newTris.Add(_faceCount * 4 + 1); //2
		_newTris.Add(_faceCount * 4 + 2); //3
		_newTris.Add(_faceCount * 4); //1
		_newTris.Add(_faceCount * 4 + 2); //3
		_newTris.Add(_faceCount * 4 + 3); //4

		float tUnit = Atlas.tUnit;

		_newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y));
		_newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y + tUnit));
		_newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y + tUnit));
		_newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y));

		_faceCount++;
	}

	public State GetState()
	{
		return _state;
	}
}
