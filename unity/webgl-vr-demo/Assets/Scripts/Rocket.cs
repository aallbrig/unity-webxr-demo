using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public ParticleSystem rocketExhaust;
    public ConstantForce force;
    public bool engineOn = false;
    [Range(1f, 100f)]
    public float explosionForce = 25f;
    [Range(0f, 5f)]
    public float explosionRange = 2f;

    private Vector3 ExplosionOrigin => transform.position + transform.up * 2;

    private List<Aircraft> _aircraftWithinRange = new List<Aircraft>();

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
    private void Update()
    {
        _aircraftWithinRange.Clear();
        var hits = Physics.OverlapSphere(ExplosionOrigin, explosionRange);
        foreach (var hit in hits)
        {
            var maybeAircraft = hit.transform.GetComponent<Aircraft>();
            if (maybeAircraft != null) _aircraftWithinRange.Add(maybeAircraft);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(ExplosionOrigin, explosionRange); 
        Gizmos.color = Color.red;
        _aircraftWithinRange.ForEach(aircraft => Gizmos.DrawLine(ExplosionOrigin, aircraft.transform.position));
    }
    private void OnCollisionEnter(Collision collision)
    {
        var aircraft = collision.transform.GetComponent<Aircraft>();
        if (aircraft) Explode();
    }
    private void Explode()
    {
        _aircraftWithinRange.ForEach(aircraft =>
        {
            var explosionDirection = (ExplosionOrigin - aircraft.transform.position).normalized * explosionForce;
            aircraft.GetComponent<ConstantForce>().enabled = false;
            aircraft.GetComponent<Rigidbody>().AddForce(explosionDirection, ForceMode.VelocityChange);
        });
        gameObject.SetActive(false);
    }
}
