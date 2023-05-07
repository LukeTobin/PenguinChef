using UnityEngine;

public class Iceberg : MonoBehaviour
{
    [SerializeField] GameObject icebergImage;

    float speed = 5;

    void Start()
    {
        icebergImage.transform.Rotate(0, 0, Random.Range(0, 360));
        speed = Random.Range(0.5f, 1.5f);
        float scaler = Random.Range(1.9f, 2.5f);
        icebergImage.transform.localScale = new Vector3(scaler, scaler, scaler);
    }

    void Update(){
        transform.position += Vector3.left * speed * Time.deltaTime; 
    }
}
