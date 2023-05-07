using UnityEngine;

[System.Serializable]
public class Reward
{
    public enum RewardType{
        bundle,
        item,
        upgrade
    }

    public RewardType rewardType = RewardType.bundle;

    public Bundle bundleReward;
    public Interactable interactableReward;
    public Upgrade upgradeReward;

    public Sprite sprite;

    public void GetReward(){
        switch(rewardType){
            case RewardType.bundle:
                GameManager.Instance.AddBundle(bundleReward);
                break;
            case RewardType.item:
                GameManager.Instance.SpawnInteractable(interactableReward.gameObject);
                break;
            default:
                break;
        }
    }
}
