/* 
 * Unless otherwise licensed, this file cannot be copied or redistributed in any format without the explicit consent of the author.
 * (c) Preet Kamal Singh Minhas, http://marchingbytes.com
 * contact@marchingbytes.com
 */
using System.Collections;
using UnityEngine;

namespace SollaraGames.ObjectPooling
{
	public class PoolObject : MonoBehaviour
	{
		[HideInInspector] public PoolObjectType _poolObjectType;
		[HideInInspector] public bool _isPooled;
	}
}