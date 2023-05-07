using UnityEngine;

public class IcebergManager : MonoBehaviour
{
    [SerializeField] GameObject icebergPrefab;
    [SerializeField] BoxCollider2D spawnArea;
    [SerializeField] int spawnCount;
    [SerializeField] BoxCollider2D resetZone;
    [SerializeField] Transform icebergParent;

    void Start(){
        for(int i = 0;i < spawnCount;i++){
            Vector2 spawnPoint = new Vector2(
                Random.Range(spawnArea.bounds.min.x, spawnArea.bounds.max.x),
                Random.Range(spawnArea.bounds.min.y, spawnArea.bounds.max.y)
            );
            Instantiate(icebergPrefab, spawnPoint, Quaternion.identity, icebergParent.transform);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        Vector2 spawnPoint = new Vector2(
            Random.Range(resetZone.bounds.min.x, resetZone.bounds.max.x),
            Random.Range(resetZone.bounds.min.y, resetZone.bounds.max.y)
        );

        other.transform.position = spawnPoint;
    }
}