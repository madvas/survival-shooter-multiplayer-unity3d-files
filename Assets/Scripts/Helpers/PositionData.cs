using UnityEngine;

public class PositionData
{
	public Vector3 position;
	public Quaternion rotation;
	
	public PositionData (Vector3 newPosition, Quaternion newRotation)
	{
		position = newPosition;
		rotation = newRotation;
	}
}