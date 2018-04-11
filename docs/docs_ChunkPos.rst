ChunkPos
********

.. default-domain:: csharp

.. class:: ChunkPos

	.. property:: int x { get; set; }
	
		X coordinate.
	
	.. property:: int y { get; set; }
	
		Y coordinate.
	
	.. property:: int z { get; set; }
	
		Z coordinate.
		
	.. property:: static ChunkPos zero { get; }
	
		Shorthand for writing ChunkPos(0, 0, 0).
	
	.. property:: static ChunkPos up { get; }
	
		Shorthand for writing ChunkPos(0, 1, 0).
	
	.. property:: static ChunkPos down { get; }
	
		Shorthand for writing ChunkPos(0, -1, 0).
	
	.. property:: static ChunkPos north { get; }
	
		Shorthand for writing ChunkPos(0, 0, 1).
	
	.. property:: static ChunkPos south { get; }
	
		Shorthand for writing ChunkPos(0, 0, -1).
	
	.. property:: static ChunkPos east { get; }
	
		Shorthand for writing ChunkPos(1, 0, 0).
	
	.. property:: static ChunkPos west { get; }
	
		Shorthand for writing ChunkPos(-1, 0, 0).

	.. method:: ChunkPos (int x, int y, int z)
	
		Creates a new :type:`ChunkPos` using explicit x, y, and z coordinates.
	
	.. method:: ChunkPos (Vector3 vec)
	
		Creates a new :type:`ChunkPos` using a Vector3.
	
	.. method:: public static float Distance (ChunkPos one, ChunkPos two)
	
		Returns the distance between two :type:`ChunkPos`.
	
	.. method:: public static float CubeDistance (ChunkPos one, ChunkPos two)
	
		Returns the cubic distance between two :type:`ChunkPos`.