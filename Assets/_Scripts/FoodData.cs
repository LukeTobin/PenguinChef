using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Food", menuName = "Food")]
public class FoodData : ScriptableObject
{
    public enum CombineType{
        Ingredient,
        Combine,
        Cook
    }

    [Header("Food Details")]
    [Tooltip("Image of the food")] public Sprite sprite;
    [Tooltip("If the food can be requested by customers")] public bool requestable = false;
    [Tooltip("If the food can be spawned for the player")] public bool spawnable = false;

    [Header("Created By")]
    public CombineType combine = CombineType.Ingredient;
    [Space]
    [ShowIf("@this.combine == CombineType.Combine || this.combine == CombineType.Cook")]
    public FoodData ingredientA;
    [ShowIf("@this.combine == CombineType.Combine")]
    public FoodData ingredientB;
}