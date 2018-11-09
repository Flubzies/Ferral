using UnityEngine;

public class AntiRotator : MonoBehaviour
{
	void LateUpdate ()
	{
		transform.rotation = Quaternion.identity;
	}
}