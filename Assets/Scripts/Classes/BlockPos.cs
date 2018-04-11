using System;

public struct BlockPos : IEquatable<BlockPos>
{
	public ChunkPos chunkPos;
	public int x, y, z;

	public static BlockPos zero = new BlockPos(ChunkPos.zero, 0, 0, 0);
	public static BlockPos up = new BlockPos(ChunkPos.zero, 0, 1, 0);
	public static BlockPos down = new BlockPos(ChunkPos.zero, 0, -1, 0);
	public static BlockPos north = new BlockPos(ChunkPos.zero, 0, 0, 1);
	public static BlockPos south = new BlockPos(ChunkPos.zero, 0, 0, -1);
	public static BlockPos east = new BlockPos(ChunkPos.zero, 1, 0, 0);
	public static BlockPos west = new BlockPos(ChunkPos.zero, -1, 0, 0);

	public BlockPos(ChunkPos chunkPos, int x, int y, int z)
	{
		this.chunkPos = chunkPos;
		this.x = x;
		this.y = y;
		this.z = z;

		Correct();
	}

	public BlockPos(int _x, int _y, int _z)
	{
		chunkPos = ChunkPos.zero;
		x = _x;
		y = _y;
		z = _z;

		Correct();
	}

	// Fancy methods

	public int GetWorldX()
	{
		return chunkPos.x * World.chunkSize + x;
	}

	public int GetWorldY()
	{
		return chunkPos.y * World.chunkSize + y;
	}

	public int GetWorldZ()
	{
		return chunkPos.z * World.chunkSize + z;
	}

	private int Mod(int x, int m)
	{
		return (x % m + m) % m;
	}

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
		return new BlockPos(lhs.chunkPos + rhs.chunkPos, lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
	}

	public static BlockPos operator -(BlockPos lhs, BlockPos rhs)
	{
		return new BlockPos(lhs.chunkPos - rhs.chunkPos, lhs.x - rhs.x, lhs.y - rhs.y, lhs.z - rhs.z);
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
