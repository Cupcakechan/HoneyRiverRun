using System;
using UnityEngine;

/// Captain Bumble's honey tank. Drains continuously; refilled by flying over Bloom Orbs.
/// Fires events so the UI (and the Phase 10 HUD) can react without the tank knowing about them.
public class HoneyTank : MonoBehaviour
{
    [Header("Tank")]
    [SerializeField] private float maxFuel = 100f;
    [SerializeField] private float drainPerSecond = 6f;   // ~16.7s from full to empty
    [SerializeField] private float refuelAmount = 40f;    // fuel added per Bloom Orb

    [Header("Warning")]
    [Range(0f, 1f)]
    [SerializeField] private float lowFuelThreshold = 0.25f;

    public float Current { get; private set; }
    public float Fraction => maxFuel <= 0f ? 0f : Current / maxFuel;
    public bool IsLow => Fraction <= lowFuelThreshold;

    /// (0..1 fraction, isLow)
    public event Action<float, bool> OnFuelChanged;
    /// Fired once when the tank hits empty.
    public event Action OnEmpty;

    private bool isEmpty;

    private void Awake() => Current = maxFuel;
    private void Start() => RaiseChanged();   // broadcast the starting full tank

    private void Update()
    {
        if (isEmpty) return;

        Current -= drainPerSecond * Time.deltaTime;
        if (Current <= 0f)
        {
            Current = 0f;
            isEmpty = true;
            RaiseChanged();
            OnEmpty?.Invoke();
            return;
        }
        RaiseChanged();
    }

    /// Fly-over refuel from a Bloom Orb.
    public void Refuel()
    {
        if (isEmpty) return;
        Current = Mathf.Min(maxFuel, Current + refuelAmount);
        RaiseChanged();
    }

    /// Reset to full (called on respawn).
    public void Refill()
    {
        Current = maxFuel;
        isEmpty = false;
        RaiseChanged();
    }

    private void RaiseChanged() => OnFuelChanged?.Invoke(Fraction, IsLow);
}