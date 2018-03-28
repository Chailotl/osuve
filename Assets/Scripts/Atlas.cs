using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atlas : MonoBehaviour
{
	public enum ID { Air, Stone, Grass, Dirt, Coal, Log, Leaves };

	public readonly static float tUnit = 0.125f;
	public enum Dir { Up, Down, North, South, East, West };

	private static Vector2[] _stone = { new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 2), new Vector2(0, 3) };
	private static Vector2[] _dirt = { new Vector2(1, 0), new Vector2(1, 1), new Vector2(1, 2), new Vector2(1, 3) };
	private static Vector2[] _grass = { new Vector2(2, 0), new Vector2(2, 1), new Vector2(2, 2) };
	private static Vector2 _grassSide = new Vector2(2, 3);
	private static Vector2[] _coal = { new Vector2(3, 0), new Vector2(3, 1), new Vector2(3, 2) };
	private static Vector2[] _log = { new Vector2(4, 0), new Vector2(4, 1) };
	private static Vector2[] _leaves = { new Vector2(3, 3), new Vector2(4, 3) };

	private static System.Random rng = new System.Random();

	public static Vector2 GetTexture(ID id, Dir dir)
	{
		switch (id)
		{
			case ID.Stone: return _stone[rng.Next(_stone.Length)];
			case ID.Grass:
				if (dir == Dir.Up) { return _grass[rng.Next(_grass.Length)]; }
				else if (dir == Dir.Down) { return _dirt[rng.Next(_dirt.Length)]; }
				else { return _grassSide; }
			case ID.Dirt: return _dirt[rng.Next(_dirt.Length)];
			case ID.Coal: return _coal[rng.Next(_coal.Length)];
			case ID.Log: return _log[rng.Next(_log.Length)];
			case ID.Leaves: return _leaves[rng.Next(_leaves.Length)];
			default: return new Vector2(0, 0);
		}
	}
}
