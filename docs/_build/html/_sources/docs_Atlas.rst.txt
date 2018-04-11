Atlas
*****

.. default-domain:: csharp

.. class:: Atlas
	
	.. property:: public static float tUnit { get; }
	
		Unit length of a cube in the texture atlas.
	
	.. method:: public Vector2 GetTexture (Atlas.ID dir, Atlas.Dir dir)
	
		Gets the texture UV coordinate for a given block ID.
	
	.. enum:: ID
	
		.. value:: Air
		.. value:: Solid
		
			A special block that represents a generic solid.
		
		.. value:: Stone
		.. value:: Grass
		.. value:: Dirt
		.. value:: Coal
		.. value:: Log
		.. value:: Leaves
	
	.. enum:: Dir
	
		.. value:: Up
		.. value:: Down
		.. value:: North
		.. value:: South
		.. value:: East
		.. value:: West