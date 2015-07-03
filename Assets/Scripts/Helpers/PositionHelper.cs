using UnityEngine;

public static class PositionHelper
{
	static Bounds floorBounds = GameObject.FindGameObjectWithTag ("Floor").GetComponent<BoxCollider> ().bounds;

	public static PositionData GetRandomSpawnPosition ()
	{
		float offset = 0;
		float randomX = Random.Range (floorBounds.center.x + (floorBounds.extents.x / 2) - offset, floorBounds.center.x - (floorBounds.extents.x / 2) + offset);
		float randomZ = Random.Range (floorBounds.center.z + (floorBounds.extents.z / 2) - offset, floorBounds.center.z - (floorBounds.extents.z / 2) + offset);
		Vector3 randomPosition = new Vector3 (randomX, 0f, randomZ);
		randomPosition = ValidateSpawnPosition (randomPosition);

		Quaternion randomRotation = Random.rotation;
		randomRotation.x = 0f;
		randomRotation.z = 0f;
		return new PositionData (randomPosition, randomRotation);
	}

	public static Transform RandomizeTransform (Transform transform)
	{
		PositionData positionData = PositionHelper.GetRandomSpawnPosition ();
		transform.position = positionData.position;
		transform.rotation = positionData.rotation;
		return transform;
	}

	static Vector3 ValidateSpawnPosition (Vector3 position)
	{
		foreach (var item in GameObject.FindGameObjectsWithTag("NotSpawnable")) {
			foreach (Collider collider in item.GetComponents<Collider>()) {
				if (collider.bounds.Contains (position)) {
					return collider.ClosestPointOnBounds (position);
				}
			}
		}
		return position;
	}
}