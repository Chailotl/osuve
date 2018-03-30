using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Chunk : MonoBehaviour
{
	private List<Vector3> _newVerts = new List<Vector3>();
	private List<int> _newTris = new List<int>();
	private List<Vector2> _newUV = new List<Vector2>();
	private List<Color> _newColors = new List<Color>();
	private int _faceCount;

	private Mesh _mesh;
	private MeshCollider _col;
	private bool _updateMesh;
	private bool _render;
	private bool _loaded;

	private int _chunkSize;
	private Int3 _chunkPos;
	private World.DataChunk _chunkData;

	public bool isolateMesh;
	private bool _updateIso;

	public void LoadData(Int3 pos, World.DataChunk chunkData)
	{
		// One-time only!
		if (!_loaded)
		{
			_loaded = true;

			_chunkSize = World.GetChunkSize();
			_chunkPos = pos;
			_chunkData = chunkData;
		}
	}

	public void SetRender(bool render)
	{
		_render = render;
	}

	public bool GetRender()
	{
		return _render;
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
			_chunkData.GenerateBlocks();
		}
	}

	public void GenerateMesh()
	{
		if (_render)
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

			_updateMesh = true;
		}
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
			Int3 pos = _chunkPos;

			if (x == -1)
			{
				x = _chunkSize - 1;
				pos = new Int3(_chunkPos.x - 1, _chunkPos.y, _chunkPos.z);
			}
			else if (x == _chunkSize)
			{
				x = 0;
				pos = new Int3(_chunkPos.x + 1, _chunkPos.y, _chunkPos.z);
			}
			else if (y == -1)
			{
				y = _chunkSize - 1;
				pos = new Int3(_chunkPos.x, _chunkPos.y - 1, _chunkPos.z);
			}
			else if (y == _chunkSize)
			{
				y = 0;
				pos = new Int3(_chunkPos.x, _chunkPos.y + 1, _chunkPos.z);
			}
			else if (z == -1)
			{
				z = _chunkSize - 1;
				pos = new Int3(_chunkPos.x, _chunkPos.y, _chunkPos.z - 1);
			}
			else if (z == _chunkSize)
			{
				z = 0;
				pos = new Int3(_chunkPos.x, _chunkPos.y, _chunkPos.z + 1);
			}
			
			return World.GetBlock(pos, x, y, z);
		}
	}

	private void UpdateMesh()
	{
		_mesh.Clear();
		_mesh.vertices = _newVerts.ToArray();
		_mesh.uv = _newUV.ToArray();
		_mesh.triangles = _newTris.ToArray();
		_mesh.colors = _newColors.ToArray();
		_mesh.RecalculateNormals();

		_col.sharedMesh = null;
		_col.sharedMesh = _mesh;

		_newVerts.Clear();
		_newUV.Clear();
		_newTris.Clear();
		_newColors.Clear();

		_faceCount = 0;
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

		if (block == Atlas.ID.Grass && Block(x, y - 1, z + 1) == Atlas.ID.Grass)
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

		if (block == Atlas.ID.Grass && Block(x, y - 1, z - 1) == Atlas.ID.Grass)
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

		if (block == Atlas.ID.Grass && Block(x + 1, y - 1, z) == Atlas.ID.Grass)
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
		
		if (block == Atlas.ID.Grass && Block(x - 1, y - 1, z) == Atlas.ID.Grass)
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
}
