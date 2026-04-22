using UnityEngine;

public class ParticleGroundStop : MonoBehaviour
{
    private ParticleSystem ps;
    private ParticleSystem.Particle[] particles;

    void Start()
    {
        ps = GetComponent<ParticleSystem>();
        particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }

    void LateUpdate()
    {
        float groundY = transform.position.y-0.1f;

        int count = ps.GetParticles(particles);

        for (int i = 0; i < count; i++)
        {
            if (particles[i].position.y <= groundY)
            {
                Vector3 p = particles[i].position;
                p.y = groundY;
                particles[i].position = p;

                particles[i].velocity = Vector3.zero;
            }
        }

        ps.SetParticles(particles, count);
    }
}