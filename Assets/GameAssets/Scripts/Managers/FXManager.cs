using UnityEngine;

public class FXManager : MonoBehaviour
{
    public void Inject() { }

    public void PlayShooterSelectedFX() { }
    public void PlayLevelCompletedFX() { }
    public void PlayLevelFailedFX() { }
}

public class ParticleSystemPool
{
    private readonly int _poolSize;
    private readonly ParticleSystem[] _particleSystems;

    private int _index;

    public ParticleSystemPool(int poolSize, ParticleSystem particleSystemPrefab, Transform parent)
    {
        _index = 0;
        _poolSize = poolSize;
        _particleSystems = new ParticleSystem[poolSize];

        for (int i = 0; i < _poolSize; i++)
        {
            _particleSystems[i] = Object.Instantiate(particleSystemPrefab, parent);
            _particleSystems[i].Stop();
        }
    }

    public ParticleSystem Play(Vector3 position)
    {
        var ps = _particleSystems[_index];
        ps.transform.position = position;
        ps.Play();

        _index = (_index + 1) % _poolSize;

        return ps;
    }
}
