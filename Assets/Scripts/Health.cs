using System;
using UnityEngine.UI;

[Serializable]
public class Health
{
    public bool IsDead => CurrentHealth <= 0;
    public float MaxHealth;
    public float CurrentHealth;
    public Slider HealthSlider;
    public Text HealthText;

    public void Update()
    {
        if (HealthSlider == null || HealthText == null) return;

        HealthSlider.value = CurrentHealth / MaxHealth;
        HealthText.text = $"{CurrentHealth} / {MaxHealth}";
        HealthSlider.transform.Find("Fill Area").gameObject.SetActive(!IsDead);
    }

    public void SetHealth(float maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
    }

    public void Damage(float damage)
    {
        if (CurrentHealth > 0)
        {
            CurrentHealth = (float)Math.Max((decimal)CurrentHealth - (decimal)damage, 0);
        }
    }
}