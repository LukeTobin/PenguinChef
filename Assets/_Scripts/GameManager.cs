using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool gameRunning = false;

    [Header("Game Settings")]
    [SerializeField] float maxTime = 40f;
    [SerializeField] TMP_Text timeText;
    float currentTime = 40f;

    [Header("Food Unlocks")]
    [SerializeField] Bundle starterBundle;
    [SerializeField] List<Bundle> unlockable = new List<Bundle>();
    [SerializeField] List<Bundle> unlocked = new List<Bundle>();

    [Header("Food Spawning")]
    [SerializeField] GameObject foodPrefab;
    [SerializeField] BoxCollider2D spawnArea;
    public FoodData rottenFood;
    [SerializeField] float delayTime = 3f;
    [SerializeField] Image foodQueuedImage;

    [Header("Effects")]
    [SerializeField] ParticleSystem combineEffect;
    [SerializeField] Sprite[] timerSprites;
    [SerializeField] SpriteRenderer queuePlaceholderObject;

    [Header("Customer Spawning")]
    [SerializeField] GameObject customerPrefab;
    [SerializeField] float customerDelayTime = 10f;
    [SerializeField] List<SpawnPoint> customerSpawnPoints = new List<SpawnPoint>();

    [Header("Rewards")]
    public List<Reward> rewards = new List<Reward>();

    List<Reward> availableRewards = new List<Reward>();

    [Header("Lists")]
    public List<FoodData> spawnableFood = new List<FoodData>();
    public List<FoodData> cookableFood = new List<FoodData>();
    public List<FoodData> creatableFood = new List<FoodData>();

    List<SpawnPoint> availablePoints = new List<SpawnPoint>();
    List<SpawnPoint> occupiedPoints = new List<SpawnPoint>();

    List<Food> spawnedFood = new List<Food>();
    List<Customer> spawnedCustomer = new List<Customer>();

    Food queuedFood;

    FoodData queuedData;
    Vector2 queuedPosition;

    // recorded stats
    [Header("Scores")]
    public int SCORE_DELIVERED = 0;
    public int SCORE_COMBINED = 0;
    public int SCORE_COOKED = 0;
    public int SCORE_ROUND = 0;
    public int SCORE_OVERALL = 0;

    void Awake(){
        Instance = this;
    }

    void Start(){
        unlocked.Add(starterBundle);
        availableRewards = new List<Reward>(rewards);

        GetSpawnableFoods();

        availablePoints = new List<SpawnPoint>(customerSpawnPoints);

        InvokeRepeating("SpawnCustomer", 3f, customerDelayTime);

        currentTime = maxTime;
        timeText.text = currentTime.ToString();

        queuePlaceholderObject.transform.DOScale(1.1f, 1f).SetLoops(-1, LoopType.Yoyo);
    }

    void Update(){
        if(!gameRunning) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timeText.text = Mathf.RoundToInt(currentTime).ToString();
        }
        else
        {
            EndRound();
        }
    }

    void EndRound(){
        currentTime = 0f;
        timeText.text = "0";
        
        SCORE_ROUND = (SCORE_DELIVERED + SCORE_COMBINED + SCORE_COOKED) * 103;
        SCORE_OVERALL += SCORE_ROUND;
        ScoreScreen.Instance.Show(SCORE_DELIVERED, SCORE_COMBINED, SCORE_COOKED, SCORE_ROUND, SCORE_OVERALL);

        SCORE_DELIVERED = 0;
        SCORE_COMBINED = 0;
        SCORE_COOKED = 0;
        SCORE_ROUND = 0;

        AudioPlayer.Instance.MenuTrack();

        gameRunning = false;
    }

    void NextRound(){
        maxTime += 15f;
        maxTime = Mathf.Clamp(maxTime, 0, 120);
        currentTime = maxTime;
        delayTime /= 2;

        foreach(Food f in spawnedFood){
            f.gameObject.SetActive(false);
        }

        foreach(Customer f in spawnedCustomer){
            f.gameObject.SetActive(false);
        }

        spawnedFood.Clear();
        spawnedCustomer.Clear();
        
        availablePoints = new List<SpawnPoint>(customerSpawnPoints);
        occupiedPoints.Clear();

        StartTimer.Instance.Reset();
    }

    public void ShowRewards(){
        List<Reward> givenRewards = new List<Reward>();
        List<Reward> tempRewardList = new List<Reward>(availableRewards);

        for(int i = 0;i < 2;i++){
            int index = Random.Range(0, tempRewardList.Count);
            givenRewards.Add(tempRewardList[index]);
            tempRewardList.RemoveAt(index);
        }
        RewardScreen.Instance.Show(givenRewards);
    }

    public void RunGame(){
        gameRunning = !gameRunning; 
        AudioPlayer.Instance.GameplayTrack();
        QueueFoodData();
    }

    public void AddBundle(Bundle newBundle){
        if(unlockable.Contains(newBundle)){
            unlocked.Add(newBundle);
            unlockable.Remove(newBundle);

            for(int i = 0;i < availableRewards.Count;i++){
                if(availableRewards[i].bundleReward = newBundle){
                    availableRewards.RemoveAt(i);
                    break;
                }
            }

            GetSpawnableFoods();
        }

        NextRound();
    }

    public void SpawnInteractable(GameObject interactableObj){
        Vector2 spawnPoint = new Vector2(
            Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
            Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y)
        );

        GameObject iObj = Instantiate(interactableObj, spawnPoint, Quaternion.identity);
        Interactable interactable = iObj.GetComponent<Interactable>();

        NextRound();
    }

    void GetSpawnableFoods(){
        spawnableFood.Clear();
        for(int i = 0;i < unlocked.Count;i++){
            spawnableFood.AddRange(unlocked[i].bundleData);
        }

        cookableFood.Clear();
        FoodData[] cookableData = Resources.LoadAll<FoodData>("Foods/Cookable");
        foreach(FoodData food in cookableData){
            if(spawnableFood.Contains(food.ingredientA)){
                cookableFood.Add(food);
            }
        }

        List<FoodData> obtainable = new List<FoodData>();
        obtainable.AddRange(spawnableFood);
        obtainable.AddRange(cookableFood);

        creatableFood.Clear();
        FoodData[] combineData = Resources.LoadAll<FoodData>("Foods/Combine");
        foreach(FoodData food in combineData){
            if(obtainable.Contains(food.ingredientA) && obtainable.Contains(food.ingredientB)){
                creatableFood.Add(food);
            }
        }
    }

    void QueueFoodData(){
        queuedData = spawnableFood[Random.Range(0, spawnableFood.Count)];
        foodQueuedImage.sprite = queuedData.sprite;
        queuedPosition =  new Vector2(
            Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
            Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y)
        );
        StartCoroutine(SpawnFoodDelay(Random.Range(1f, 4f)));
        queuePlaceholderObject.gameObject.transform.position = queuedPosition;
        queuePlaceholderObject.sprite = timerSprites[0];
    }

    void SpawnFood(){
        if(!gameRunning) return;

        GameObject foodObject = Instantiate(foodPrefab, queuedPosition, Quaternion.identity);
        Food food = foodObject.GetComponent<Food>();

        food.Spawn(queuedData);
        
        foodObject.name = food.Data.name;
        spawnedFood.Add(food);

        QueueFoodData();
    }

    IEnumerator SpawnFoodDelay(float timeExpected){
        float elapsedTime = 0f; // The amount of time elapsed so far
        int currentSpriteIndex = 0;
        while (elapsedTime < timeExpected)
        {
            if(!gameRunning){
                StopCoroutine("SpawnFoodDelay");
            }

            // Calculate the index of the sprite to display
            int spriteIndex = Mathf.FloorToInt(elapsedTime / timeExpected * timerSprites.Length);

            // If the index has changed, update the sprite
            if (spriteIndex != currentSpriteIndex)
            {
                currentSpriteIndex = spriteIndex;
                queuePlaceholderObject.sprite = timerSprites[currentSpriteIndex];
            }

            // Wait for the next frame
            yield return null;

            // Update the elapsed time
            elapsedTime += Time.deltaTime;
        }

        SpawnFood();
    }

    public void CombineQueue(Food food){
        if(queuedFood != null){
            Combine(queuedFood, food);
            queuedFood = null;
        }
        else{
            queuedFood = food;
        }
    }

    void Combine(Food foodA, Food foodB){
        FoodData combined = null;

        for(int i = 0;i < creatableFood.Count;i++){
            if(foodA.Data == creatableFood[i].ingredientA && foodB.Data == creatableFood[i].ingredientB){
                combined = creatableFood[i];
                break;
            }
            else if(foodA.Data == creatableFood[i].ingredientB && foodB.Data == creatableFood[i].ingredientA){
                combined = creatableFood[i];
                break;
            }
        }

        if(!combined) return;
        
        ParticleSystem pfx = Instantiate(combineEffect, foodA.gameObject.transform.position, Quaternion.identity);
        pfx.Play();

        SCORE_COMBINED++;

        foodA.Change(combined);
        foodB.gameObject.SetActive(false);
    }
    
    void SpawnCustomer(){
        if(availablePoints.Count <= 0 || !gameRunning) return;

        int index = Random.Range(0, availablePoints.Count);
        SpawnPoint spawnPoint = availablePoints[index];
        occupiedPoints.Add(availablePoints[index]);

        GameObject customerObject = Instantiate(customerPrefab, (Vector2)spawnPoint.gameObject.transform.position, Quaternion.identity);
        Customer customer = customerObject.GetComponent<Customer>();
        customer.Spawn(creatableFood[Random.Range(0, creatableFood.Count)], spawnPoint);

        availablePoints.RemoveAt(index);
        spawnedCustomer.Add(customer);
    }

    public void Unoccupy(SpawnPoint point, bool happy){
        if(happy) SCORE_DELIVERED++;
        if(occupiedPoints.Contains(point)){
            int index = occupiedPoints.IndexOf(point);
            availablePoints.Add(occupiedPoints[index]);
            occupiedPoints.RemoveAt(index);
        }
    }
}
