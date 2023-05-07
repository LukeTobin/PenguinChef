using UnityEngine;
using TMPro;
using DG.Tweening;

public class ScoreScreen : MonoBehaviour
{
    public static ScoreScreen Instance;

    [SerializeField] GameObject scoreSection;

    [Header("Score Texts")]
    [SerializeField] TMP_Text ordersText;
    [SerializeField] TMP_Text combinedText;
    [SerializeField] TMP_Text cookedText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text overallText;

    bool active = false;

    void Awake(){
        Instance = this;
    }

    void Start(){
        scoreSection.SetActive(false);
        active = false;
    }

    public void Show(int orders, int combined, int cooked, int score, int overall){
        scoreSection.SetActive(true);
        active = true;

        TweenText(ordersText, orders, 0.8f);
        TweenText(combinedText, combined, 0.9f);
        TweenText(cookedText, cooked, 1f);
        TweenText(scoreText, score, 1.1f);
        TweenText(overallText, overall, 1.4f);
    }

    void TweenText(TMP_Text tmp, int value, float timeToTake){
        int startValue = 0;
        DOTween.To(() => startValue, x => {
            startValue = x;
            tmp.text = x.ToString();
        }, value, timeToTake);
    }

    void Update(){
        if(!active) return;

        if(Input.GetKeyDown(KeyCode.Space)){
            scoreSection.SetActive(false);
            active = false;
            GameManager.Instance.ShowRewards();
        }
    }
}
