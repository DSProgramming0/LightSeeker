using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectController : MonoBehaviour
{
    [SerializeField] private ParticleSystem currentParticleEffect;
    [SerializeField] private ParticleSystem.MinMaxCurve originalStartSpeed;
    [SerializeField] private ParticleSystem.MinMaxCurve originalStartSize;
    [SerializeField] private ParticleSystem.MinMaxCurve originalEmmissionRate;

    // Start is called before the first frame update
    void Start()
    {
        originalStartSpeed = currentParticleEffect.main.startSpeed;  //stores particle systems original values locally
        originalStartSize = currentParticleEffect.main.startSize;
        originalEmmissionRate = currentParticleEffect.emission.rateOverTime;

        GameEvents.instance.onCinematicTriggerEnter += particleIncreaseCinematic;
        GameEvents.instance.onCinematicTriggerExit += resetParticleSystemValues;
    }   

    public void particleIncreaseCinematic()
    {
        setParticleSystemValues(1f, 2f, .1f, .5f, 10000f);
    }

    public void setParticleSystemValues(float _minSpeed, float _maxSpeed, float _minSize, float _maxSize, float _rateOverTime) //Sets particle system values to aggressive state
    {
        var s = currentParticleEffect.main;
        var e = currentParticleEffect.emission;
        s.startSpeed = new ParticleSystem.MinMaxCurve(_minSpeed, _maxSpeed);
        s.startSize = new ParticleSystem.MinMaxCurve(_minSize, _maxSize);
        e.rateOverTime = _rateOverTime;
    }

    public void resetParticleSystemValues() //Resets particle system values to orginal state
    {
        var s = currentParticleEffect.main;
        var e = currentParticleEffect.emission;

        s.startSpeed = originalStartSpeed;
        s.startSize = originalStartSize;
        e.rateOverTime = originalStartSpeed;
    }
}
