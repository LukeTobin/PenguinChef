using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardCard : MonoBehaviour
{
    Button m_Button;
    [SerializeField] Image rewardImage;
    [SerializeField] TMP_Text rewardText;

    Reward reward;

    void Start(){
        m_Button = GetComponent<Button>();
        m_Button.onClick.AddListener(Select);
    }

    public void Create(Reward _reward){
        reward = _reward;
        switch(_reward.rewardType){
            case Reward.RewardType.bundle:
                rewardImage.sprite = _reward.bundleReward.coverSprite;
                rewardText.text = _reward.bundleReward.name + " Bundle!";
                break;
            case Reward.RewardType.item:
                rewardImage.sprite = _reward.sprite;
                rewardText.text = _reward.interactableReward.name;
                break;
        }
    }
    
    void Select(){
        RewardScreen.Instance.Selected(reward);
    }
}
