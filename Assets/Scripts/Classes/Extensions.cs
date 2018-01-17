using UnityEngine;

public static class Extensions	{
	/// <summary>
	/// Called to find a component on a child that has the given tag
	/// </summary>
	/// <returns>The component in child with tag.</returns>
	/// <param name="parent">Parent.</param>
	/// <param name="tag">Tag.</param>
	/// <typeparam name="T">The 1st type parameter.</typeparam>
	public static T FindComponentInChildWithTag<T>(this GameObject parent, string tag)where T:Component{
		Transform t = parent.transform;
		foreach(Transform tr in t) {
			if(tr.tag == tag) {
				return tr.GetComponent<T>();
			}
		}

		return null;
	}

	/// <summary>
	/// Called to find a game object of a child with the given tag
	/// </summary>
	/// <returns>The child with tag.</returns>
	/// <param name="parent">Parent.</param>
	/// <param name="tag">Tag.</param>
	public static GameObject FindChildWithTag(this GameObject parent, string tag) {
		Transform t = parent.transform;
		foreach(Transform tr in t) {
			if(tr.tag == tag) {
				return tr.gameObject;
			}
		}

		return null;
	}
}

