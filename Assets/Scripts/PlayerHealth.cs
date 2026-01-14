using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("HP Settings")]
    [SerializeField] private int maxHP = 3;
    private int currentHP;

    [Header("UI")]
    [SerializeField] private Slider hpSlider;

    void Start()
    {
        currentHP = maxHP;

        if (hpSlider != null)
        {
            hpSlider.maxValue = maxHP;
            hpSlider.value = currentHP;
        }
    }

    // ★ ダメージを受ける
    public void TakeDamage(int damage)
    {
        currentHP -= damage;
        currentHP = Mathf.Max(0, currentHP);

        if (hpSlider != null)
        {
            hpSlider.value = currentHP;
        }

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GameManager.instance.hart = 0;
    }
}
