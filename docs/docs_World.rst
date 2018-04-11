World
*****

.. default-domain:: csharp

.. class:: World

	.. property:: GameObject _chunkPrefab { }
	.. property:: Dictionary<ChunkPos, DataChunk> _chunks { }
	.. property:: Dictionary<ColumnPos, DataColumn> _columns { }
	.. property:: Dictionary<ChunkPos, DataChunk> _offloacChunks { }
	.. property:: SimplePriorityQueue<Chunk> _loadQueue { }
	.. property:: bool _rendering { }
	.. property:: static int _viewRangeHorizontal { }
	.. property:: static int _viewRangeVertical { }
	.. property:: static ChunkPos _playerPos { }
	
	.. property:: public int chunkSize { get; }
	
		The size (width, breadth, and height) of chunks.
	
	.. method:: public static Atlas.ID GetBlock (BlockPos pos)
	
		Fetches a block that has already been generated in the past.
	
	.. method:: public static Atlas.ID GenerateBlock (BlockPos pos)
	
		Generates a block for a given position.
	
	.. method:: public static int GenerateTopology (int x, int z)
	
		Generates the topological height of the stone layer for a given coordinate.
	
	.. method:: public static float SimplexNoise (float x, float y, float z, float scale, float height, float power)
	
		Returns simplex noise from a given point, modulated by some variables.
	
	.. method:: public static int GetViewRange ()
	
		Gets view range.
	
	.. method:: public static DataChunk GetChunk (ChunkPos pos)
	
		Gets the :type:`DataChunk` from the given pos.
	
	.. method:: public static DataColumn GetColumn (ColumnPos pos)
	
		Gets the :type:`DataColumn` from the given pos.
	
	.. method:: void RenderThread ()
	
		Special function that does threaded generation.
	
	.. method:: void GenerateChunks ()
	
		Generates all possible chunk positions that are in view range if a chunk does not already exist at that position.
	
	.. method:: void PingChunks ()
	
		Checks whether chunks are still in view range or not, and destrys them if need be.
	
	.. method:: void DestroyChunk (ChunkPos pos)
	
		Safely removes a :type:`Chunk` from the chunk dictionary.