using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atlas : MonoBehaviour
{
	public enum ID : ushort { Air, Solid, Stone, Grass, Dirt, Coal, Log, Leaves };

	public static Dictionary<string, Color> Colors = new Dictionary<string, Color>();

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

	void Start()
	{
		Colors["Tropical_1"] = new Color(67f / 255f, 146f / 255f, 42f / 255f);
		Colors["Tropical_2"] = new Color(51f / 255f, 112f / 255f, 32f / 255f);
		Colors["Tropical_3"] = new Color(44f / 255f, 95f / 255f, 27f / 255f);
		Colors["Tropical_4"] = new Color(31f / 255f, 69f / 255f, 20f / 255f);

		Colors["Normal_1"] = new Color(82f / 255f, 149f / 255f, 47f / 255f);
		Colors["Normal_2"] = new Color(64f / 255f, 116f / 255f, 37f / 255f);
		Colors["Normal_3"] = new Color(58f / 255f, 106f / 255f, 34f / 255f);
		Colors["Normal_4"] = new Color(53f / 255f, 97f / 255f, 31f / 255f);

		Colors["Temperate_1"] = new Color(85f / 255f, 138f / 255f, 65f / 255f);
		Colors["Temperate_2"] = new Color(77f / 255f, 126f / 255f, 60f / 255f);
		Colors["Temperate_3"] = new Color(62f / 255f, 102f / 255f, 48f / 255f);
		Colors["Temperate_4"] = new Color(56f / 255f, 90f / 255f, 43f / 255f);

		Colors["Chaparral_1"] = new Color(106f / 255f, 143f / 255f, 63f / 255f);
		Colors["Chaparral_2"] = new Color(85f / 255f, 114f / 255f, 50f / 255f);
		Colors["Chaparral_3"] = new Color(68f / 255f, 92f / 255f, 40f / 255f);
		Colors["Chaparral_4"] = new Color(56f / 255f, 76f / 255f, 34f / 255f);

		Colors["Savanna_1"] = new Color(123f / 255f, 121f / 255f, 60f / 255f);
		Colors["Savanna_2"] = new Color(104f / 255f, 103f / 255f, 51f / 255f);
		Colors["Savanna_3"] = new Color(94f / 255f, 93f / 255f, 46f / 255f);
		Colors["Savanna_4"] = new Color(74f / 255f, 73f / 255f, 36f / 255f);

		Colors["Tundra_1"] = new Color(110f / 255f, 141f / 255f, 86f / 255f);
		Colors["Tundra_2"] = new Color(94f / 255f, 121f / 255f, 73f / 255f);
		Colors["Tundra_3"] = new Color(78f / 255f, 100f / 255f, 61f / 255f);
		Colors["Tundra_4"] = new Color(58f / 255f, 74f / 255f, 45f / 255f);
	}

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
