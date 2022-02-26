using UnityEngine;

public class Rocket : MonoBehaviour
{
    public ParticleSystem rocketExhaust;
    public ConstantForce force;
    public bool engineOn = false;
    public void ToggleEngine()
    {
        if (!engineOn) TurnOnEngine();
        else TurnOffEngine();
    }
    private void Awake()
    {
        if (engineOn) TurnOnEngine();
        else TurnOffEngine();
    }
    private void TurnOffEngine()
    {
        engineOn = false;
        force.enabled = false;
        rocketExhaust.Stop();
    }
    private void TurnOnEngine()
    {
        engineOn = true;
        force.enabled = true;
        rocketExhaust.Play();
    }
}
