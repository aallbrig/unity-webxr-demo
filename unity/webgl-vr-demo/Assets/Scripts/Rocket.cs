using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public ParticleSystem rocketExhaust;
    public ConstantForce force;
    public bool engineOn;
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
    public void TurnOnEngine()
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
        _aircraftWithinRange.ForEach(aircraft =>
        {
            var heading = aircraft.transform.position - ExplosionOrigin;
            var normalizedDirection = heading / heading.magnitude;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(ExplosionOrigin, normalizedDirection * explosionRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(ExplosionOrigin, (ExplosionOrigin - aircraft.transform.position).normalized * (explosionRange * 2));
        });
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
            var explosionForceDirection = (ExplosionOrigin - aircraft.transform.position).normalized * explosionForce;
            var rigidBody = aircraft.GetComponent<Rigidbody>();
            aircraft.GetComponent<ConstantForce>().enabled = false;
            rigidBody.AddForce(explosionForceDirection, ForceMode.Impulse);
        });
        gameObject.SetActive(false);
    }
}
