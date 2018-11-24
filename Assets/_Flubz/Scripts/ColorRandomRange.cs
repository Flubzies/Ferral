using System.Collections.Generic;
using UnityEngine;

public class ColorRandomRange : MonoBehaviour
{
	[SerializeField] Color _minColor;
	[SerializeField] Color _maxColor;
	[SerializeField] bool _skinnedMesh;
	[SerializeField] bool _applyToAllMaterials;

	private void Start ()
	{
		Vector3 _minCol;
		Vector3 _maxCol;

		Color.RGBToHSV (_minColor, out _minCol.x, out _minCol.y, out _minCol.z);
		Color.RGBToHSV (_maxColor, out _maxCol.x, out _maxCol.y, out _maxCol.z);
		Color c = Random.ColorHSV (_minCol.x, _maxCol.x, _minCol.y, _maxCol.y, _minCol.z, _maxCol.z, _minColor.a, _maxColor.a);

		if (_skinnedMesh)
		{
			if (_applyToAllMaterials)
				foreach (Material item in GetComponent<SkinnedMeshRenderer> ().materials) item.color = c;
			else GetComponent<SkinnedMeshRenderer> ().material.color = c;
		}
		else
		{
			if (_applyToAllMaterials)
				foreach (Material item in GetComponent<MeshRenderer> ().materials) item.color = c;
			else GetComponent<MeshRenderer> ().material.color = c;
		}
	}
}