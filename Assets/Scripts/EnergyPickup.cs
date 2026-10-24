using UnityEngine;

// Vật thể năng lượng: khi nhân vật chạm vào (Trigger) thì cộng điểm và biến mất.
[RequireComponent(typeof(Collider))]
public class EnergyPickup : MonoBehaviour
{
    private bool collected;

    private void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Cờ collected + tắt collider ngay để không bao giờ tính điểm hai lần cho cùng một vật thể.
        if (collected || !other.CompareTag("Player"))
            return;

        collected = true;
        GetComponent<Collider>().enabled = false;

        if (GameManager.Instance != null)
            GameManager.Instance.AddEnergy();

        Destroy(gameObject);
    }
}
