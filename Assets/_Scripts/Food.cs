using System.Collections;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Food : MonoBehaviour
{
    public FoodData Data {get;private set;}

    [SerializeField] LayerMask collideLayer;

    [Header("Audio")]
    [SerializeField] AudioClip combineSFX;
    [SerializeField] AudioClip collisionSFX;

    [Header("Cooking")]
    [SerializeField] Slider cookSlider;

    public bool cooking = false;

    [SerializeField] float cookSpeed = 1.0f;
    [SerializeField] float coolSpeed = 2.0f;

    private bool isFilling = false;
    private bool isEmptying = false;

    private float currentValue = 0.0f;
    public FoodData Cook {get;set;}

    // Private
    SpriteRenderer spriteRenderer;
    AudioSource audioSource;
    Sequence sequence;

    void Awake(){
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        ResetSeq();

        cookSlider.value = 0;
        currentValue = 0;
        cookSlider.gameObject.SetActive(false);
    }

    void Update(){
        if (cooking && !isFilling) {
            StartCoroutine(FillSlider());
        }
        else if (!cooking && !isEmptying && cookSlider.value > 0.0f) {
            StartCoroutine(EmptySlider());
        }

        if(cookSlider.isActiveAndEnabled){
            if(cookSlider.value <= 0 && !cooking){
                Debug.Log("disable");
                cookSlider.gameObject.SetActive(false);
                currentValue = 0;
            } 
        }
        else if(cookSlider.value > 0 && !cookSlider.isActiveAndEnabled){
            cookSlider.gameObject.SetActive(true);
        }
        
    }

    IEnumerator FillSlider () {
        isFilling = true;
        
        cookSlider.gameObject.SetActive(true);

        while (currentValue < cookSlider.maxValue) {
            if(!cooking || !cookSlider.isActiveAndEnabled) {
                StopCoroutine(FillSlider());
            }

            currentValue += cookSpeed * Time.deltaTime;
            cookSlider.value = currentValue;

            yield return null;
        }
        
        cookSlider.value = 0;
        currentValue = 0;

        isFilling = false;
        isEmptying = false;

        GameManager.Instance.SCORE_COOKED++;

        Change(Cook);

        Cook = null;
        cooking = true;
    }

    IEnumerator EmptySlider () {
        isEmptying = true;

        cookSlider.gameObject.SetActive(true);

        while (currentValue > 0.0f) {
            if(cooking) {
                StopCoroutine(EmptySlider());
            }

            if(!cookSlider.isActiveAndEnabled){
                Debug.Log("resetting 1");
                cookSlider.gameObject.SetActive(true);
            }
            currentValue -= coolSpeed * Time.deltaTime;
            cookSlider.value = currentValue;
            if (currentValue <= 0.0f) {
                cookSlider.gameObject.SetActive(false);
                isEmptying = false;
                yield break;
            }
            yield return null;
        }

        isEmptying = false;
        cookSlider.value = 0;
        cookSlider.gameObject.SetActive(false);
        Cook = null;
    }

    public void Spawn(FoodData data){
        Data = data;
        spriteRenderer.sprite = data.sprite;
    }

    public void Change(FoodData data){
        if(data == null) data = GameManager.Instance.rottenFood;

        Data = data;
        spriteRenderer.sprite = data.sprite;
        sequence.Restart();

        if(!gameObject.activeInHierarchy) return;

        audioSource.clip = combineSFX;
        audioSource.time = 0;
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.Play();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.IsTouchingLayers(collideLayer)){
            GameManager.Instance.CombineQueue(this);
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(!gameObject.activeInHierarchy) return;

        audioSource.clip = collisionSFX;
        audioSource.time = 0;
        audioSource.pitch = Random.Range(0.6f, 1.1f);
        audioSource.Play();
    }

    void ResetSeq(){
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOScale(0.9f, 0.1f));
        sequence.Append(transform.DOScale(1.1f, 0.2f));
        sequence.Append(transform.DOScale(1f, 0.1f));
        sequence.SetAutoKill(false); // prevent sequence from being killed automatically
        sequence.Pause(); // pause the sequence initially
    }
}
