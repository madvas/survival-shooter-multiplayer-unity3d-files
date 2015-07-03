using UnityEngine;
using System.Collections;

public class PlayerNetworking : Photon.MonoBehaviour
{
	float timer;                   
	bool initialLoad = true;
	Vector3 position;
	Quaternion rotation;
	float smoothing = 10f;
	Animator anim;
	bool isWalking = false;

	PlayerHealth playerHealth;
	int IsWalkingHash = Animator.StringToHash ("IsWalking");
	int respawnHash = Animator.StringToHash ("Respawn");

	void Awake ()
	{
		playerHealth = GetComponent<PlayerHealth> ();
		anim = GetComponent<Animator> ();
	}

	void Start ()
	{
		if (!photonView.isMine) {
			StartCoroutine (UpdateData ());
		} else {
			ResetScore ();
		}
	}

	IEnumerator UpdateData ()
	{
		if (initialLoad) {
			initialLoad = false;
			transform.position = position;
			transform.rotation = rotation;
		}
		
		while (true) {
			transform.position = Vector3.Lerp (transform.position, position, Time.deltaTime * smoothing);
			transform.rotation = Quaternion.Lerp (transform.rotation, rotation, Time.deltaTime * smoothing);
			anim.SetBool ("IsWalking", isWalking);
			yield return null;
		}
	}

	void OnPhotonSerializeView (PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting) {
			stream.SendNext (transform.position);
			stream.SendNext (transform.rotation);
			stream.SendNext (anim.GetBool (IsWalkingHash));
		} else {
			position = (Vector3)stream.ReceiveNext ();
			rotation = (Quaternion)stream.ReceiveNext ();
			isWalking = (bool)stream.ReceiveNext ();
		}
	}

	[PunRPC]
	public void Shoot (Vector3 hitPositon)
	{
		DrawShot (gunEndTransform.position, hitPositon);
	}

	[PunRPC]
	public void TakeShot (int damage, int shooterId, Vector3 hitPositon)
	{
		bool died;
		playerHealth.TakeDamage (damage, hitPositon, out died);
		if (died) {
			if (photonView.isMine) {
				photonView.owner.AddDeaths (1);
			}

			if (shooterId == PhotonNetwork.player.ID) {
				PhotonNetwork.player.AddScore (1);
			}
			PhotonPlayer killer = PhotonPlayer.Find (shooterId);
			GameObjectHelper.SendMessageToAll ("OnPlayerKill", killer, photonView.owner);
		}
	}

	[PunRPC]
	public void RespawnPlayer ()
	{
		anim.SetTrigger (respawnHash);
	}

	void OnPlayerRespawn ()
	{
		if (photonView.isMine) {
			photonView.RPC ("RespawnPlayer", PhotonTargets.All);
		}
	}

	void OnRoundStarted ()
	{
		ResetScore ();
	}

	void ResetScore ()
	{
		if (photonView.isMine) {
			photonView.owner.SetScore (0);
			photonView.owner.SetDeaths (0);
		}
	}

	void OnPlayerDamageChange (object[] changeData)
	{
		int increasedDamage = (int)changeData [0];
		int bonusDuration = (int)changeData [1];
		
		CancelInvoke ("ResetShotEffects");
		Invoke ("ResetShotEffects", bonusDuration);
	}
}
