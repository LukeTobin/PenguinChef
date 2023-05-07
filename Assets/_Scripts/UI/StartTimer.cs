using System.Collections;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class StartTimer : MonoBehaviour
{
    public static StartTimer Instance;

    [SerializeField] GameObject timerObject;
    [SerializeField] TMP_Text timerText;
    [SerializeField] int timeRemaining = 3;

    bool started = false;

    void Awake(){
        Instance = this;
    }

    void Start(){
        Reset();
    }

    public void Reset(){
        timerText.text = "Press Space To Start";
        timerText.gameObject.SetActive(true);
        timerObject.SetActive(true);
        started = false;
    }

    void Update(){
        if(started) return;

        if(Input.GetKeyDown(KeyCode.Space)){
            StartCountdown();
            started = true;
        }
    }

    public void StartCountdown(){
        StartCoroutine(CountdownCoroutine());
    }

    IEnumerator CountdownCoroutine()
    {
        while (timeRemaining > 0)
        {
            timerText.text = timeRemaining.ToString();

            yield return new WaitForSeconds(1f);

            timeRemaining--;

            timerText.gameObject.transform.DOScale(new Vector3(0.9f, 0.9f, 1f), 0.5f)
                .SetEase(Ease.InOutQuad)
                .OnComplete(() =>
                {
                    timerText.gameObject.transform.DOScale(new Vector3(1.1f, 1.1f, 1f), 0.5f)
                        .SetEase(Ease.InOutQuad);
                });
        }

        timerObject.SetActive(false);
        timerText.text = "Go!";

        GameManager.Instance.RunGame();

        yield return new WaitForSeconds(0.5f);

        timerText.gameObject.SetActive(false);
    }
}
