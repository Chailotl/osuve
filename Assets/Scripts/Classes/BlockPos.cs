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

	public BlockPos(ChunkPos _chunkPos, int _x, int _y, int _z)
	{
		chunkPos = _chunkPos;
		x = _x;
		y = _y;
		z = _z;

		// Offset chunk
		int _chunkSize = World.GetChunkSize();
		chunkPos = chunkPos + new ChunkPos( (int) Math.Floor(x / (float)_chunkSize), (int) Math.Floor(y / (float)_chunkSize), (int) Math.Floor(z / (float)_chunkSize) );

		// Clamp blocks
		x = x % _chunkSize;
		y = y % _chunkSize;
		z = z % _chunkSize;
	}

	public BlockPos(int _x, int _y, int _z)
	{
		chunkPos = ChunkPos.zero;
		x = _x;
		y = _y;
		z = _z;

		// Offset chunk
		int _chunkSize = World.GetChunkSize();
		chunkPos = chunkPos + new ChunkPos((int)Math.Floor(x / (float)_chunkSize), (int)Math.Floor(y / (float)_chunkSize), (int)Math.Floor(z / (float)_chunkSize));

		// Clamp blocks
		x = x % _chunkSize;
		y = y % _chunkSize;
		z = z % _chunkSize;
	}

	// Fancy methods

	public int GetWorldX()
	{
		return chunkPos.x * World.GetChunkSize() + x;
	}

	public int GetWorldY()
	{
		return chunkPos.y * World.GetChunkSize() + y;
	}

	public int GetWorldZ()
	{
		return chunkPos.z * World.GetChunkSize() + z;
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
		return new BlockPos(lhs.x + rhs.x, lhs.y + rhs.y, lhs.z + rhs.z);
	}

	public static bool operator ==(BlockPos lhs, BlockPos rhs)
	{
		return (lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z);
	}

	public static bool operator !=(BlockPos lhs, BlockPos rhs)
	{
		return (lhs.x != rhs.x || lhs.y != rhs.y || lhs.z != rhs.z);
	}

	/*public static implicit operator BlockPos(BlockPos rhs)
	{
		return new BlockPos(rhs.x, rhs.z);
	}*/
}
