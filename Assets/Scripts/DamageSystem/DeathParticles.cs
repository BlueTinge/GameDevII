using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//most of this code taken from https://www.reddit.com/r/Unity3D/comments/50dayf/make_particles_track_to_object/

public class DeathParticles : MonoBehaviour
{
    public ParticleSystem p;
    public ParticleSystem.Particle[] particles;
    public Transform Target;
    public Vat Vat;
    public float SpeedIncrement;
    public float MaxSpeed;
    public float HoverTime;
    public float KillDistance;

    // Start is called before the first frame update
    void Start()
    {
        Vat = GameObject.FindGameObjectWithTag("Vat").GetComponent<Vat>();
        if (Vat != null)
        {
            Target = Vat.Emitter1.transform;
            StartCoroutine(UpdateParticles());
        }
    }

    // Update is called once per frame
    IEnumerator UpdateParticles()
    {

        yield return new WaitForSeconds(HoverTime);

        var e = p.emission;
        e.enabled = false;

        float speed = 0;

        particles = new ParticleSystem.Particle[p.particleCount];
        p.GetParticles(particles);

        bool endLoop = false;
        while (!endLoop && !Vat.isDestroyed)
        {
            endLoop = true;
            p.GetParticles(particles);

            if (speed < MaxSpeed) speed += SpeedIncrement;

            for (int i = 0; i < particles.GetUpperBound(0); i++)
            {
                Vector3 ActualPos = (particles[i].position + transform.position);

                //print(Vector3.Distance(Target.position, ActualPos));

                if (Vector3.Distance(Target.position, ActualPos) > KillDistance && particles[i].remainingLifetime > 0)
                {
                    particles[i].velocity += (Target.position - ActualPos).normalized * speed;
                    particles[i].remainingLifetime = 1f;
                }
                else
                {
                    particles[i].remainingLifetime = 0;
                }

                if(particles[i].remainingLifetime > 0)
                {
                    endLoop = false; //dont end loop as long as one particle is still alive
                }

            }

            p.SetParticles(particles, particles.Length);

            yield return new WaitForFixedUpdate();

        }


        yield return new WaitForSeconds(3);

        Destroy(gameObject);

    }
}
