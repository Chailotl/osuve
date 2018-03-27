using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atlas : MonoBehaviour
{
	public enum ID { Air, Stone, Grass, Dirt, Ore, Log, Leaves };

	//private static int _lenID = System.Enum.GetNames(typeof(ID)).Length;
	//private static Vector2[] Tex = new Vector2[_lenID];

	public readonly static float tUnit = 0.25f;
	public enum Dir { Up, Down, North, South, East, West };

	private static Vector2 _stone = new Vector2(0, 0);
	private static Vector2 _dirt = new Vector2(1, 0);
	private static Vector2 _grass = new Vector2(2, 0);
	private static Vector2 _ore = new Vector2(3, 0);
	private static Vector2 _log = new Vector2(0, 1);
	private static Vector2 _leaves = new Vector2(1, 1);

	public static Vector2 GetTexture(ID id, Dir dir)
	{
		switch (id)
		{
			case ID.Stone: return _stone;
			case ID.Grass:
				if (dir == Dir.Up) { return _grass; }
				else { return _dirt; }
			case ID.Ore: return _ore;
			case ID.Log: return _log;
			case ID.Leaves: return _leaves;
			default: return new Vector2(0, 0);
		}
	}
}
