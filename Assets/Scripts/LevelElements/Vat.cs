using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Vat : MonoBehaviour
{
    private HealthStats VatHealth;
    public bool isDestroyed = false;
    private bool isShaking = false;

    private Vector3 StartPos;

    public ParticleSystem Emitter1;
    public ParticleSystem Emitter2;

    public float freq = 1f;
    public float amp = .15f;
    public int numShakeFrames = 60;

    public AudioSource audio;
    public AudioClip vatbreak;
    public AudioClip vathit;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        audio.clip = vatbreak;
        VatHealth = GetComponent<HealthStats>();
        VatHealth.OnDeath = OnDeath;
        VatHealth.OnKnockback = OnKnockback;
        VatHealth.OnDamage = delegate (float damage) { };
        VatHealth.OnImmunityEnd = delegate () { };

        StartPos = transform.position;

        if (Manager.GetCheckpoint(SceneManager.GetActiveScene().name))
        {
            VatHealth.CurrentHealth = -1;
        }
    }

    //take damage: shake
    public void OnKnockback(Vector3 knockback)
    {
        StartCoroutine(Shake(knockback));
    }

    //is destroyed: release particles, set checkpoint
    public void OnDeath(float overkill)
    {
        if (!isDestroyed)
        {
            isDestroyed = true;
            Manager.SetCheckpoint(SceneManager.GetActiveScene().name, true);
            StartCoroutine(ReleaseParticles());
        }
    }

    IEnumerator Shake(Vector3 knockback)
    {
        audio.Play();


        if (isShaking) yield break;
        isShaking = true; //TODO: make this more thread safe

        transform.position = StartPos;

        for (int i = 0; i < numShakeFrames; i++)
        {
            //transform.position = startPos;
            transform.Translate(knockback.normalized*(Mathf.Sin(i*freq) * amp * (((numShakeFrames-i)*1.0f) / numShakeFrames)  ));

            yield return new WaitForFixedUpdate();
        }

        VatHealth.isImmune = false;
        isShaking = false;
    }

    IEnumerator switchsound()
    {
        yield return new WaitForSeconds(1);
        audio.clip = vathit;
    }

    IEnumerator ReleaseParticles()
    {
        StartCoroutine("switchsound");
        Emitter1.Play();
        Emitter2.Play();
        yield return new WaitForSeconds(Emitter1.main.startLifetime.constant + 1);
        Emitter1.Stop();
        Emitter2.Stop();
    }
}
