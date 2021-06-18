using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePool : MonoBehaviour
{

    public ParticleData[] particles;
    public int poolSize = 4;

    private Dictionary<string, PooledParticle> pool;
    
    void Start()
    {

        pool = new Dictionary<string, PooledParticle>();

        for(int i = 0; i < particles.Length; i++)
        {
            ParticleData data = particles[i];
            PooledParticle pooledParticle = new PooledParticle();
            GameObject[] tmp = new GameObject[poolSize];
            for(int i2 = 0; i2 < poolSize; i2++)
            {
                GameObject instance = Instantiate(data.particle);
                instance.transform.position = Vector3.zero;
                instance.transform.localScale = Vector3.one;
                tmp[i2] = instance;
            }
            pooledParticle.Objects = tmp;
            pool.Add(data.id, pooledParticle);
        }

    }

    public GameObject Pool(string id)
    {
        PooledParticle particle = pool[id];
        particle.index++;
        return particle.Objects[particle.index % poolSize];
    }

}

public class ParticleData
{
    public string id;
    public GameObject particle;

}

public class PooledParticle
{

    public GameObject[] Objects;
    public int index = -1;
}