using UnityEngine;

public class Loader : MonoBehaviour
{
    [SerializeField] GameObject playerObj;

    void Awake()
    {
        int players = PlayerPrefs.GetInt("Players", 1);
        
        GameObject p = Instantiate(playerObj);
        Player pl = p.GetComponent<Player>();
        pl.playerNumber = 0;

        if(players > 1){
            Debug.Log("greater than 1");
            GameObject p2 = Instantiate(playerObj);
            Player pl2 = p.GetComponent<Player>();
            pl2.playerNumber = 1;
        }
    }
}
