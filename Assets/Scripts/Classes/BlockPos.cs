using System;

public struct BlockPos : IEquatable<BlockPos>
{
	/// <summary>The chunk this block is located in.</summary>
	public ChunkPos chunkPos;
	/// <summary>The local coordinate of this block.</summary>
	public int x, y, z;

	/// <summary>Shorthand for writing <c>BlockPos(0, 0, 0)</c>.</summary>
	public readonly static BlockPos zero = new BlockPos(0, 0, 0);
	/// <summary>Shorthand for writing <c>BlockPos(0, 1, 0)</c>.</summary>
	public readonly static BlockPos up = new BlockPos(0, 1, 0);
	/// <summary>Shorthand for writing <c>BlockPos(0, -1, 0)</c>.</summary>
	public readonly static BlockPos down = new BlockPos(0, -1, 0);
	/// <summary>Shorthand for writing <c>BlockPos(0, 0, 1)</c>.</summary>
	public readonly static BlockPos north = new BlockPos(0, 0, 1);
	/// <summary>Shorthand for writing <c>BlockPos(0, 0, -1)</c>.</summary>
	public readonly static BlockPos south = new BlockPos(0, 0, -1);
	/// <summary>Shorthand for writing <c>BlockPos(1, 0, 0)</c>.</summary>
	public readonly static BlockPos east = new BlockPos(1, 0, 0);
	/// <summary>Shorthand for writing <c>BlockPos(-1, 0, 0)</c>.</summary>
	public readonly static BlockPos west = new BlockPos(-1, 0, 0);

	/// <summary>
	/// Create a new <c>BlockPos</c> with <paramref name="x"/>, <paramref name="y"/>, and <paramref name="z"/> coordinates inside a ChunkPos.
	/// </summary>
	/// <param name="x">Local x coordinate.</param>
	/// <param name="y">Local y coordinate.</param>
	/// <param name="z">Local z coordinate.</param>
	/// <param name="chunkPos">Chunk position.</param>
	/// <remarks>Coordinate overflow will offset <c>chunkPos</c>.</remarks>
	public BlockPos(int x, int y, int z, ChunkPos chunkPos)
	{
		this.chunkPos = chunkPos;
		this.x = x;
		this.y = y;
		this.z = z;

		Correct();
	}

	/// <summary>
	/// Create a new <c>BlockPos</c> with <paramref name="x"/>, <paramref name="y"/>, and <paramref name="z"/> coordinates; ChunkPos.zero is assumed.
	/// </summary>
	/// <param name="x">Global x coordinate.</param>
	/// <param name="y">Global y coordinate.</param>
	/// <param name="z">Global z coordinate.</param>
	/// <remarks><c>chunkPos</c> will be automatically set.</remarks>
	public BlockPos(int x, int y, int z)
	{
		chunkPos = ChunkPos.zero;
		this.x = x;
		this.y = y;
		this.z = z;

		Correct();
	}
	
	/// <summary>
	/// Returns global x coordinate.
	/// </summary>
	/// <returns>Global x coordinate.</returns>
	public int GetWorldX()
	{
		return chunkPos.x * World.chunkSize + x;
	}

	/// <summary>
	/// Returns global y coordinate.
	/// </summary>
	/// <returns>Global y coordinate.</returns>
	public int GetWorldY()
	{
		return chunkPos.y * World.chunkSize + y;
	}

	/// <summary>
	/// Returns global z coordinate.
	/// </summary>
	/// <returns>Global z coordinate.</returns>
	public int GetWorldZ()
	{
		return chunkPos.z * World.chunkSize + z;
	}

	/// <summary>
	/// The default C# modulus operator <c>%</c> returns negative numbers for negative input, which is undesirable.
	/// </summary>
	/// <param name="x">Input.</param>
	/// <param name="m">Modulus.</param>
	/// <returns>Always-positive output.</returns>
	private int Mod(int x, int m)
	{
		return (x % m + m) % m;
	}

	/// <summary>
	/// This corrects overflown coordinates, appropriately shifting <c>chunkPos</c> and clamping <c>x</c>, <c>y</c>, and <c>z</c>.
	/// </summary>
	private void Correct()
	{
		// Offset chunk
		chunkPos = chunkPos + new ChunkPos((int)Math.Floor(x / (float)World.chunkSize), (int)Math.Floor(y / (float)World.chunkSize), (int)Math.Floor(z / (float)World.chunkSize));

		// Clamp blocks
		x = Mod(x, World.chunkSize);
		y = Mod(y, World.chunkSize);
		z = Mod(z, World.chunkSize);
	}

	// Base methods

	public override string ToString()
	{
		return "Block: (" + x + ", " + y + ", " + z + ")";
	}

	public bool Equals(BlockPos other)
	{
		return (this.x == other.x && this.y == other.y && this.z == other.z);
	}

	public override bool Equals(object obj)
	{
		return base.Equals(obj);
	}

	public override int GetHashCode()
	{
		var hashCode = 373119288;
		hashCode = hashCode * -1521134295 + base.GetHashCode();
		hashCode = hashCode * -1521134295 + x.GetHashCode();
		hashCode = hashCode * -1521134295 + y.GetHashCode();
		hashCode = hashCode * -1521134295 + z.GetHashCode();
		return hashCode;
	}

	// Operators

	public static BlockPos operator +(BlockPos lhs, BlockPos rhs)
	{
		return new BlockPos(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z, lhs.chunkPos + rhs.chunkPos);
	}

	public static BlockPos operator -(BlockPos lhs, BlockPos rhs)
	{
		return new BlockPos(lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z, lhs.chunkPos - rhs.chunkPos);
	}

	public static bool operator ==(BlockPos lhs, BlockPos rhs)
	{
		return (lhs.chunkPos == rhs.chunkPos && lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z);
	}

	public static bool operator !=(BlockPos lhs, BlockPos rhs)
	{
		return (lhs.chunkPos != rhs.chunkPos && lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z);
	}

	public static implicit operator ChunkPos(BlockPos rhs)
	{
		return rhs.chunkPos;
	}
}
