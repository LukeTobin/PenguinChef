using UnityEngine;

public class Customer : MonoBehaviour
{
    public FoodData Data {get;private set;}

    [Header("Colours")]
    [SerializeField] Color defaultColor = Color.green;
    [SerializeField] Color angryColor = Color.red;
    [SerializeField] Color happyColor = Color.cyan;

    [Header("Sprites")]
    [SerializeField] SpriteRenderer charSR;
    [SerializeField] SpriteRenderer requestSR;
    [SerializeField] SpriteRenderer ingred1;
    [SerializeField] SpriteRenderer ingred2;

    SpawnPoint point;

    public void Spawn(FoodData data, SpawnPoint _point){
        Data = data;
        this.point = _point;

        requestSR.sprite = data.sprite;
        if(data.combine == FoodData.CombineType.Combine){
            ingred1.sprite = data.ingredientA.sprite;
            ingred1.enabled = true;
            ingred2.sprite = data.ingredientB.sprite;
            ingred2.enabled = true;
        }
        else{
            ingred1.sprite = null;
            ingred1.enabled = false;
            ingred2.sprite = null;
            ingred2.enabled = false;
        }
    }

    void Despawn(){
        GameManager.Instance.Unoccupy(point, charSR.color == happyColor);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.GetComponent<Food>()){
            if(other.GetComponent<Food>().Data == Data){
                charSR.color = happyColor;
                Invoke("Despawn", 1f);
            }
            else{
                charSR.color = angryColor;
                Invoke("Despawn", 1f);
            }

            other.gameObject.SetActive(false);
        }
    }

}
