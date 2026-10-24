using System;
using UnityEngine;

// Quản lý điểm năng lượng và trạng thái thắng/thua của màn chơi.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int totalEnergy = 5;

    public int CollectedEnergy { get; private set; }
    public int TotalEnergy => totalEnergy;
    public bool IsGameOver { get; private set; }
    public bool IsWon { get; private set; }

    // Màn chơi đã kết thúc (thắng hoặc thua): nhân vật và kẻ địch dừng lại.
    public bool IsFinished => IsGameOver || IsWon;

    // (đã thu, tổng) — UI cập nhật dòng "Energy: x/5".
    public event Action<int, int> EnergyChanged;

    // Nhặt đủ năng lượng — UI hiện "YOU WIN".
    public event Action Won;

    // Nhân vật chạm kẻ địch — UI hiện "GAME OVER".
    public event Action GameOver;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void AddEnergy()
    {
        if (IsFinished)
            return;

        CollectedEnergy++;
        Debug.Log($"Energy: {CollectedEnergy}/{totalEnergy}");
        EnergyChanged?.Invoke(CollectedEnergy, totalEnergy);

        if (CollectedEnergy >= totalEnergy)
        {
            IsWon = true;
            Debug.Log("YOU WIN");
            Won?.Invoke();
        }
    }

    public void TriggerGameOver()
    {
        if (IsFinished)
            return;

        IsGameOver = true;
        Debug.Log("GAME OVER");
        GameOver?.Invoke();
    }
}
