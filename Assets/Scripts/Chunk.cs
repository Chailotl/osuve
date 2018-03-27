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
	private int _faceCount;

	private Mesh _mesh;
	private MeshCollider _col;
	private bool _updateMesh;
	public bool isolateMesh;
	private bool _updateIso;

<<<<<<< HEAD
	public void LoadData(World.Int3 pos, World.DataChunk chunkData)
	{
		// One-time only!
		if (!_loaded)
		{
			_loaded = true;

			_chunkX = pos.x;
			_chunkY = pos.y;
			_chunkZ = pos.z;
=======
	public int chunkX;
	public int chunkY;
	public int chunkZ;
>>>>>>> parent of 83ba107... Better chunk data security

	public World.DataChunk chunkData;

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

	public void GenerateMesh()
	{
		// Check if data chunk blocks are generated
		if (!chunkData.IsGenerated())
		{
			chunkData.GenerateBlocks();
		}

		int chunkSize = World.GetChunkSize();

		// Iterate through x, y, z
		for (int x = 0; x < chunkSize; x++)
		{
			for (int y = 0; y < chunkSize; y++)
			{
				for (int z = 0; z < chunkSize; z++)
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

	// Local block to world blocks
	private Atlas.ID Block(int x, int y, int z)
	{
		if (x >= 0 && x <= 15 && y >= 0 && y <= 15 && z >= 0 && z <= 15)
		{
			// In bounds, we have the data available to us
			return chunkData.blocks[x, y, z];
		}
		else if (isolateMesh)
		{
			return Atlas.ID.Air;
		}
		else
		{
			// Outside of bounds, need to fetch
			return World.GenerateBlock(x + chunkX, y + chunkY, z + chunkZ);
			//return World.Block(chunkX, chunkY, chunkZ, x, y, z);
		}
	}

	private void UpdateMesh()
	{
		_mesh.Clear();
		_mesh.vertices = _newVerts.ToArray();
		_mesh.uv = _newUV.ToArray();
		_mesh.triangles = _newTris.ToArray();
		_mesh.RecalculateNormals();

		_col.sharedMesh = null;
		_col.sharedMesh = _mesh;

		_newVerts.Clear();
		_newUV.Clear();
		_newTris.Clear();

		_faceCount = 0;
	}

	private void CubeUp(int x, int y, int z, Atlas.ID block)
	{
		_newVerts.Add(new Vector3(x, y, z + 1));
		_newVerts.Add(new Vector3(x + 1, y, z + 1));
		_newVerts.Add(new Vector3(x + 1, y, z));
		_newVerts.Add(new Vector3(x, y, z));

		Vector2 texturePos = Atlas.GetTexture(block, Atlas.Dir.Up);

		Cube(texturePos);
	}

	private void CubeDown(int x, int y, int z, Atlas.ID block)
	{
		_newVerts.Add(new Vector3(x, y - 1, z));
		_newVerts.Add(new Vector3(x + 1, y - 1, z));
		_newVerts.Add(new Vector3(x + 1, y - 1, z + 1));
		_newVerts.Add(new Vector3(x, y - 1, z + 1));

		Vector2 texturePos = Atlas.GetTexture(block, Atlas.Dir.Down);

		Cube(texturePos);
	}

	private void CubeNorth(int x, int y, int z, Atlas.ID block)
	{
		_newVerts.Add(new Vector3(x + 1, y - 1, z + 1));
		_newVerts.Add(new Vector3(x + 1, y, z + 1));
		_newVerts.Add(new Vector3(x, y, z + 1));
		_newVerts.Add(new Vector3(x, y - 1, z + 1));

		Vector2 texturePos = Atlas.GetTexture(block, Atlas.Dir.North);

		Cube(texturePos);
	}

	private void CubeSouth(int x, int y, int z, Atlas.ID block)
	{
		_newVerts.Add(new Vector3(x, y - 1, z));
		_newVerts.Add(new Vector3(x, y, z));
		_newVerts.Add(new Vector3(x + 1, y, z));
		_newVerts.Add(new Vector3(x + 1, y - 1, z));

		Vector2 texturePos = Atlas.GetTexture(block, Atlas.Dir.South);

		Cube(texturePos);
	}

	private void CubeEast(int x, int y, int z, Atlas.ID block)
	{
		_newVerts.Add(new Vector3(x + 1, y - 1, z));
		_newVerts.Add(new Vector3(x + 1, y, z));
		_newVerts.Add(new Vector3(x + 1, y, z + 1));
		_newVerts.Add(new Vector3(x + 1, y - 1, z + 1));

		Vector2 texturePos = Atlas.GetTexture(block, Atlas.Dir.East);

		Cube(texturePos);
	}

	private void CubeWest(int x, int y, int z, Atlas.ID block)
	{
		_newVerts.Add(new Vector3(x, y - 1, z + 1));
		_newVerts.Add(new Vector3(x, y, z + 1));
		_newVerts.Add(new Vector3(x, y, z));
		_newVerts.Add(new Vector3(x, y - 1, z));

		Vector2 texturePos = Atlas.GetTexture(block, Atlas.Dir.West);

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
