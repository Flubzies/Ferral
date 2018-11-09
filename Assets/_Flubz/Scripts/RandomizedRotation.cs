using DG.Tweening;
using UnityEngine;

public class RandomizedRotation : MonoBehaviour
{
	[SerializeField] float _rotChangeTimer = 2.0f;
	[SerializeField] Ease _ease;

	private void Start ()
	{
		RandomRotation ();
	}

	void RandomRotation ()
	{
		Tweener t = transform.DORotateQuaternion (Random.rotation, _rotChangeTimer);
		t.SetEase (_ease);
		t.OnComplete (RandomRotation);
	}
}