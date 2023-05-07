using System.Collections.Generic;
using UnityEngine;

public class Cooker : Interactable
{
    List<FoodData> cookable = new List<FoodData>();
    List<FoodData> cookableIngredients = new List<FoodData>();

    List<Food> cookingFood = new List<Food>();

    FoodData defaultFood;

    void Awake(){
        FoodData[] data = Resources.LoadAll<FoodData>("Foods/Cookable");

        foreach(FoodData food in data){
            if(food.combine == FoodData.CombineType.Cook) cookable.Add(food);
        }

        for(int i = 0;i < cookable.Count;i++){
            cookableIngredients.Add(cookable[i].ingredientA);
        }
    }

    void Start(){
        defaultFood = GameManager.Instance.rottenFood;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.GetComponent<Food>()){
            Food food = other.GetComponent<Food>();
            FoodData cooking = defaultFood;

            if(cookableIngredients.Contains(food.Data)){
                cooking = cookable[cookableIngredients.IndexOf(food.Data)];
            }

            cookingFood.Add(food);
            food.cooking = true;
            food.Cook = cooking;
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if(other.GetComponent<Food>()){
            Food food = other.GetComponent<Food>();
            if(cookingFood.Contains(food)){
                food.cooking = false;
                food.Cook = null;
                cookingFood.Remove(food);
            }
        }
    }
}
