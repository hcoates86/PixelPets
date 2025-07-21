using UnityEngine;

public class Item : MonoBehaviour
{
    public GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (Data.Instance.gameManager != null)
            gameManager = Data.Instance.gameManager;
        else
            Data.Instance.RefreshReferences();

        //adds itself to dirty items on creation
        AddSelfToDirty();
    }

    void AddSelfToDirty()
    {
        gameManager.dirtyFloorItems.Add(gameObject);
    }
}
