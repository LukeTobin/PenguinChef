using System.Collections.Generic;
using UnityEngine;

public class RewardScreen : MonoBehaviour
{
    public static RewardScreen Instance;

    [SerializeField] GameObject rewardSection;
    [SerializeField] Transform layoutGroup;
    [SerializeField] GameObject rewardCardPrefab;

    List<RewardCard> rewardCards = new List<RewardCard>();

    void Awake(){
        Instance = this;
    }

    void Start(){
        rewardSection.SetActive(false);
    }

    public void Selected(Reward reward){
        // do reward
        reward.GetReward();

        Close();
    }

    public void Show(List<Reward> rewards){
        for(int i = 0;i < rewards.Count;i++){
            GameObject rewardObj = Instantiate(rewardCardPrefab);
            rewardObj.transform.SetParent(layoutGroup, false);
            RewardCard rewardCard = rewardObj.GetComponent<RewardCard>();
            rewardCard.Create(rewards[i]);
            rewardCards.Add(rewardCard);
        }

        rewardSection.SetActive(true);
    }

    public void Close(){
        foreach(RewardCard obj in rewardCards){
            obj.gameObject.SetActive(false);
        }

        rewardCards.Clear();

        rewardSection.SetActive(false);
    }
}
