using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bundle", menuName = "Bundle")]
public class Bundle : ScriptableObject
{
    public Sprite coverSprite;
    public List<FoodData> bundleData = new List<FoodData>();
}
