using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;

public class GameManager : MonoBehaviour
{
    public GameObject newChar;

    public CharacterAI characterAI;

    public GameObject exertionObj;
    public GameObject foodDish;
    public Transform foodPos;
    public List<GameObject> dirtyFloorItems;
    public List<GameObject> prettyFloorItems;
    public List<GameObject> cleanableNeutralFloorItems;
    public List<GameObject> funItems;

    public Food food;

    private float newXPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Data.Instance.RefreshReferences();
        
    }

    public void Feed()
    {
        
        newXPos = UnityEngine.Random.Range(-2f, 2f);
        food = Instantiate(foodDish, new Vector3(foodPos.position.x + newXPos, foodPos.position.y, foodPos.position.z), Quaternion.identity).GetComponent<Food>();

        if(Data.Instance.currentChar.hasDied) return;
            StartCoroutine(Data.Instance.currentChar.DisplayEmotion(EmotionList.Happy));

        if (characterAI.aiOn)
        {
            characterAI.StopAllCoroutines();
            StartCoroutine(characterAI.MoveToTargetAndEat(food.transform));
        }
    }

    public void Clean()
    {
        //destroys everything in list then makes a new, clean list yay
        foreach (GameObject item in dirtyFloorItems)
        {
            Destroy(item);
            
        }
        // if there are any leftover items that weren't also in the dirty items list, destroy them
        if (cleanableNeutralFloorItems.Count > 0)
        {
            foreach (GameObject item in cleanableNeutralFloorItems)
            {
                Destroy(item);
                
            }
        }

        dirtyFloorItems = new List<GameObject>();
        cleanableNeutralFloorItems = new List<GameObject>();

    }

    public void HaveFun()
    {
        Data.Instance.currentChar.fun += 100;
        int randomIndex = UnityEngine.Random.Range(0, funItems.Count);
        newXPos = UnityEngine.Random.Range(-2f, 2f);

        Instantiate(funItems[randomIndex], new Vector3(Data.Instance.currentChar.transform.position.x + newXPos, 3.5f, Data.Instance.currentChar.transform.position.z), Quaternion.identity);
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

public static class Extensions
{
	public static List<T> GetComponentsRecursive<T>(this GameObject gameObject) where T : Component
	{
		int length = gameObject.transform.childCount;
		List<T> components = new List<T>(length + 1);
		T comp = gameObject.transform.GetComponent<T>();
		if (comp != null) components.Add(comp);
		for (int i = 0; i < length; i++)
		{
			comp = gameObject.transform.GetChild(i).GetComponent<T>();
			if (comp != null) components.Add(comp);
		}
		return components;
	}
	
	public static List<T> GetComponentsInDirectChildren<T>(this GameObject gameObject) where T : Component
	{
		int length = gameObject.transform.childCount;
		List<T> components = new List<T>(length);
		for (int i = 0; i < length; i++)
		{
			T comp = gameObject.transform.GetChild(i).GetComponent<T>();
			if (comp != null) components.Add(comp);
		}
		return components;
	}
}
