using UnityEngine;

public static class PositionHelper
{
	static GameObject floor = GameObject.FindGameObjectWithTag ("Floor");
	static Bounds floorBounds = GameObject.FindGameObjectWithTag ("Floor").GetComponent<BoxCollider> ().bounds;

	public static PositionData GetRandomSpawnPosition ()
	{
		float offset = 0;
		float randomX = Random.Range (floorBounds.center.x + (floorBounds.extents.x / 2) - offset, floorBounds.center.x - (floorBounds.extents.x / 2) + offset);
		float randomZ = Random.Range (floorBounds.center.z + (floorBounds.extents.z / 2) - offset, floorBounds.center.z - (floorBounds.extents.z / 2) + offset);
		Vector3 randomPosition = new Vector3 (randomX, 0, randomZ);
		randomPosition = ValidateSpawnPosition (randomPosition);

		Quaternion randomRotation = Random.rotation;
		return new PositionData (randomPosition, randomRotation);
	}

	static Vector3 ValidateSpawnPosition (Vector3 position)
	{
		foreach (var item in GameObject.FindGameObjectsWithTag("NotSpawnable")) {
			foreach (Collider collider in item.GetComponents<Collider>()) {
				if (collider.bounds.Contains (position)) {
					return collider.ClosestPointOnBounds (Vector3.zero);
				}
			}
		}
		return position;
	}
}