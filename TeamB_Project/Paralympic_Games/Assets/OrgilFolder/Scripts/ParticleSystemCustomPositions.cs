using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSystemCustomPositions : MonoBehaviour
{
    private ParticleSystem _particleSystem;
    [SerializeField] private Transform[] positions;

    private IEnumerator Start()
    {
        yield return null;

        _particleSystem = GetComponent<ParticleSystem>();
        var particles = new ParticleSystem.Particle[_particleSystem.main.maxParticles];
        int count = _particleSystem.GetParticles(particles);
        for (int i = 0; i < positions.Length; i++)
        {
            particles[i].position = positions[i].position;
            particles[i].startSize = 2f;
            particles[i].startLifetime = 1000f;

        }
        _particleSystem.SetParticles(particles, count);

    }

}
