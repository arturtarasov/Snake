using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Element
{
	public GameObject Prefab;
	public ElementType ElementType;
	[SerializeField]
	public List<Obj> SpawnedObjects;
}
