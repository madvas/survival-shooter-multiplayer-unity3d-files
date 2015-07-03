using UnityEngine;
using System.Collections;

public class IPickable : MonoBehaviour
{

	public interface IKillable
	{
		void Kill ();
	}
}
