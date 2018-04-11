Chunk
*****

.. default-domain:: csharp

.. class:: Chunk
	
	.. property:: List<Vector3> _newVerts { }
	.. property:: List<int> _newTris { }
	.. property:: List<Vector2> _newUV { }
	.. property:: List<Color> _newColors { }
	.. property:: int _faceCount { }
	.. property:: Mesh _mesh { }
	.. property:: MeshCollider _col { }
	.. property:: bool _updateMesh { }
	.. property:: bool _clearMesh { }
	.. property:: State _state { }
	.. property:: int _chunkSize { }
	.. property:: ChunkPos _chunkPos { }
	.. property:: DataChunk _chunkData { }
	
	.. method:: public void LoadData (ChunkPos pos, DataChunk chunkData)
	
		Loads data into chunk.
	
	.. method:: public void UpdateState ()
	
		Lets chunk know it can properly update its state.
	
	.. method:: public void GenerateBlocks ()
	
		Tells chunk to generate its blocks.
	
	.. method:: public void GenerateMesh ()
	
		Tells chunk to generate its mesh.
	
	.. method public State GetState ()
	
		Get current state of chunk.
	
	.. method:: Atlas.ID Block (BlockPos pos)
	
		Get block from position
	
	.. method:: UpdateMesh ()
	
		Updates mesh.
	
	.. method:: void CubeUp (int x, int y, int z, Atlas.ID block)
	.. method:: void CubeDown (int x, int y, int z, Atlas.ID block)
	.. method:: void CubeNorth (int x, int y, int z, Atlas.ID block)
	.. method:: void CubeSouth (int x, int y, int z, Atlas.ID block)
	.. method:: void CubeEast (int x, int y, int z, Atlas.ID block)
	.. method:: void CubeWest (int x, int y, int z, Atlas.ID block)
	.. method:: void Cube (Vector2 texturePos)
	
	.. enum:: State
	
		.. value:: Fresh
		
			The chunk has been freshly created, but it has no data associated.
		
		.. value:: Prepped
		
			The chunk now has its basic data.
		
		.. value:: Generating
		
			The chunk is actively generating or retrieving block data.
		
		.. value:: Loaded
		
			The chunk has its block data loaded.
		
		.. value:: Rendered
		
			The chunk is actively rendering.