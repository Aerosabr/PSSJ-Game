using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    [SerializeField] private List<GameObject> nearbyObjects = new List<GameObject>();
	[SerializeField] private GameObject closestItem;
	[SerializeField] private Coroutine coroutine;

	public void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log("Enter")
        if (collision.gameObject.tag == "Wolf")
            transform.GetChild(0).gameObject.SetActive(true);
		if (collision.gameObject.tag == "Item")
            nearbyObjects.Add(collision.gameObject);
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
		//Debug.Log("Exit");
		if (collision.gameObject.tag == "Wolf")
			transform.GetChild(0).gameObject.SetActive(false);
		if (collision.gameObject.tag == "Item")
            nearbyObjects.Remove(collision.gameObject);
    }

	public void OnTriggerStay2D(Collider2D collision)
	{
		HighlightNearest();
	}

	public void OnPickup()
    {
		int index = 0;
		bool containSpace = false;
        if (nearbyObjects.Count > 1)
        {
            float distance = Mathf.Infinity;
            for (int i = 0; i < nearbyObjects.Count; i++)
            {
                if (Vector3.Distance(transform.position, nearbyObjects[i].transform.position) < distance)
                {
                    
                    distance = Vector3.Distance(transform.position, nearbyObjects[i].transform.position);
                    index = i;
                }
            }
			containSpace = nearbyObjects[index].GetComponent<Item>().PickupItem();
        }
        else if (nearbyObjects.Count == 1)
			containSpace = nearbyObjects[index].GetComponent<Item>().PickupItem();
		if(!containSpace)
		{
			nearbyObjects[index].transform.GetChild(1).gameObject.SetActive(true);
			if (coroutine != null)
				StopCoroutine(coroutine);
			coroutine = StartCoroutine(DeactivateAfterDelay(nearbyObjects[index].transform.GetChild(1).gameObject, 2.0f));
		}
    }

	public void HighlightNearest()
	{
		if(closestItem != null)
		{
			closestItem.transform.GetChild(0).gameObject.SetActive(false);
		}
		if (nearbyObjects.Count > 0)
		{
			float distance = Mathf.Infinity;
			int index = 0;
			for (int i = 0; i < nearbyObjects.Count; i++)
			{
				if (Vector3.Distance(transform.position, nearbyObjects[i].transform.position) < distance)
				{

					distance = Vector3.Distance(transform.position, nearbyObjects[i].transform.position);
					index = i;
				}
			}
			closestItem = nearbyObjects[index];
			closestItem.transform.GetChild(0).gameObject.SetActive(true);
		}
	}

	private IEnumerator DeactivateAfterDelay(GameObject obj, float delay)
	{
		// Wait for 'delay' seconds
		yield return new WaitForSeconds(delay);

		// Deactivate the GameObject
		obj.SetActive(false);
	}
}
