using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerEnergy : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private Canvas canvas;
    private Slider EnergyBar;
    private TMP_Text EnergyText;

    [Header("Energy")]
    [SerializeField] private float maxEnergy = 100f;
    [SerializeField] private float energyGainMultiplier = 1;
    [SerializeField] private float maxEnergyMultiplier = 1;

    private float energy = 100f;

    [Header("Energy Loss")]
    [SerializeField] private float energyLossRate = 0.1f;

    [Range(0, 1)]
    [SerializeField] private float energySmoothening = 0.1f;

    private bool isDead = false;

    void Start()
    {
        energy = maxEnergy;
        EnergyBar = canvas.GetComponentInChildren<Slider>();
        EnergyBar.maxValue = maxEnergy;
        EnergyBar.value = energy;

        EnergyText = canvas.GetComponentInChildren<TMP_Text>();
    }

    void Update()
    {
        if (energy > 0)
        {
            if(energy < 0)
                energy = 0;

            energy -= energyLossRate * Time.deltaTime;
            EnergyBar.value = Mathf.Lerp(EnergyBar.value, energy, energySmoothening);

            EnergyText.text = "Energy: " + Mathf.Round(energy) + "/" + Mathf.Round(maxEnergy * maxEnergyMultiplier);
        }

        if (energy <= 0)
        {
            energy = 0;
            Die();
        }
    }

    public void GainEnergy(float amount)
    {
        energy += amount * energyGainMultiplier;
        if (energy > maxEnergy * maxEnergyMultiplier)
            energy = maxEnergy * maxEnergyMultiplier;
    }

    public void LoseEnergy(float amount)
    {
        energy -= amount;
    }

    void Die()
    {
        if (!isDead)
        {
            isDead = true;
            GetComponent<PlayerController>().StartCoroutine("Death");
        }
    }

}
