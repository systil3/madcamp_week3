using System;
using TMPro;
using UnityEngine.UI;

[Serializable]
public class Health
{
    public bool IsDead => CurrentHealth <= 0;
    public float MaxHealth;
    public float CurrentHealth;
    public Slider HealthSlider;
    public TextMeshProUGUI HealthText;

    public void Update()
    {
        if (HealthSlider == null || HealthText == null) return;

        HealthSlider.value = CurrentHealth / MaxHealth;
        HealthText.text = $"{CurrentHealth} / {MaxHealth}";
        HealthSlider.transform.Find("Fill Area").gameObject.SetActive(!IsDead);
    }

    public void SetMaxHealth(float maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
    }

    public void Heal(float heal)
    {
        CurrentHealth = (float)Math.Min((decimal)CurrentHealth + (decimal)heal, (decimal)MaxHealth);
    }

    public void Damage(float damage)
    {
        CurrentHealth = (float)Math.Max((decimal)CurrentHealth - (decimal)damage, 0);
    }
}