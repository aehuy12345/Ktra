using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Giao diện: hiển thị "Energy: x/5", màn hình YOU WIN / GAME OVER và nút Play Again.
public class GameUI : MonoBehaviour
{
    [SerializeField] private Text energyText;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject gameOverPanel;

    private GameManager manager;

    private void Start()
    {
        winPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        manager = GameManager.Instance;
        if (manager == null)
        {
            Debug.LogWarning("[GameUI] Không tìm thấy GameManager trong Scene");
            return;
        }

        manager.EnergyChanged += UpdateEnergy;
        manager.Won += ShowWin;
        manager.GameOver += ShowGameOver;
        UpdateEnergy(manager.CollectedEnergy, manager.TotalEnergy);
    }

    private void OnDestroy()
    {
        if (manager == null)
            return;
        manager.EnergyChanged -= UpdateEnergy;
        manager.Won -= ShowWin;
        manager.GameOver -= ShowGameOver;
    }

    private void UpdateEnergy(int collected, int total)
    {
        energyText.text = $"Energy: {collected}/{total}";
    }

    private void ShowWin() => winPanel.SetActive(true);

    private void ShowGameOver() => gameOverPanel.SetActive(true);

    // Gắn vào nút Play Again: tải lại Scene hiện tại.
    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
