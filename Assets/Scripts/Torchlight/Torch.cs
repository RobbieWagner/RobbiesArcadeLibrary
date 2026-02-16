using System;
using RobbieWagnerGames.Utilities;
using UnityEngine;

public class Torch : MonoBehaviourSingleton<Torch>
{
    public Vector3 windDirection = Vector3.right;
    public Transform torchlight;
    public float minTilt = 10f;
    public float maxTilt = 25f;
    private Vector3 initialLightScale;
    
    private float currentTorchlightTilt = 0f;
    private float targetTilt = 0f;

    [field: SerializeField] public Timer snuffTimer {get; private set;}
    [SerializeField] private float snuffTime = 180f;

    public static event Action<Torch> OnInitializeTorch = null;

    protected override  void Awake()
    {
        base.Awake();

        SetRandomTilt();
        initialLightScale = torchlight.localScale;

        snuffTimer.OnTimerComplete += SnuffTorch;
    }

    public void InitializeTorch()
    {
        snuffTimer.StartTimer(snuffTime);
        OnInitializeTorch?.Invoke(this);
    }

    public void SetRandomTilt()
    {
        targetTilt = UnityEngine.Random.Range(minTilt, maxTilt);
    }

    public void SetWindDirection(Vector3 newWindDirection)
    {
        windDirection = newWindDirection.normalized;
        SetRandomTilt();
    }

    public void TiltTorchlight(float magnitudeDelta)
    {
        targetTilt += magnitudeDelta;
        targetTilt = Mathf.Clamp(targetTilt, minTilt, maxTilt);
    }

    public void SetTorchlightTilt(float magnitude)
    {
        targetTilt = Mathf.Clamp(magnitude, minTilt, maxTilt);
    }

    public void ResetTorchlight()
    {
        targetTilt = 0;
        transform.localScale = initialLightScale;
        snuffTimer.ResetTimer();
    }

    private void Update()
    {
        currentTorchlightTilt = Mathf.Lerp(currentTorchlightTilt, targetTilt, Time.deltaTime * 5f);
        Vector3 rotationAxis = Vector3.Cross(windDirection, Vector3.up).normalized;
        torchlight.rotation = Quaternion.AngleAxis(currentTorchlightTilt, rotationAxis);

        if (snuffTimer != null && snuffTimer.duration > 0)
        {
            float t = Mathf.Clamp01(snuffTimer.timerValue / snuffTimer.duration);
            float scaleFactor = Mathf.Lerp(1f, 0.3f, Mathf.Pow(t, 1.5f));
            
            if (initialLightScale == Vector3.zero)
                initialLightScale = torchlight.localScale;
            
            torchlight.localScale = initialLightScale * scaleFactor;
        }
    }

    private void SnuffTorch()
    {
        torchlight.gameObject.SetActive(false);
        //Play extinguish sound
        //Game over
    }
}