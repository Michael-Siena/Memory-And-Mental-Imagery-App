using UnityEngine;
using System.Collections;

public class SpriteOcclusion : MonoBehaviour 
{
	public int sortingOrd = -490;

	// This ensures that the crosshair always renders behind all other sprites
	public void Update() => GetComponent<SpriteRenderer>().sortingOrder = sortingOrd;
}
