using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Data : MonoBehaviour
{
    public static Data Instance { get; private set; }

    public Character currentChar;
    public CharacterAI characterAI;

    public GameManager gameManager;
    public int totalCharsDied = 0;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            //if this isn't the instance, pass current basic references
            PassReferences();
            Destroy(this.gameObject); // Destroy other instance
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RefreshReferences();
    }

    public void CharacterDied()
    {
        characterAI.StopAllCoroutines();
        currentChar.StopAllCoroutines();
        currentChar.player.StopAllCoroutines();
        characterAI.isMoving = false;
    }


    void PassReferences()
    {
        if (Data.Instance.gameManager == null)
            Data.Instance.gameManager = gameManager;
    }

    public void RefreshReferences()
    {
        if (gameManager == null)
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

    }
}
