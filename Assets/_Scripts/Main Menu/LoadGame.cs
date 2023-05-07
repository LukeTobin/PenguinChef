using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadGame : MonoBehaviour
{
    Button m_Button;

    public int playerCount = 0;

    void Awake(){
        m_Button = GetComponent<Button>();
        m_Button.onClick.AddListener(Click);
    }

    void Click(){
        PlayerPrefs.SetInt("Players", playerCount);
        PlayerPrefs.Save();
        
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }
}
