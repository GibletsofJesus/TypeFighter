﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExplosionManager : MonoBehaviour {

    public static ExplosionManager instance;
    public GameObject smallExplosionPrefab, mediumExplosionPrefab, bigExplosionPrefab;
    List<Explosion> smallExploList = new List<Explosion>();
    List<Explosion> mediumExploList = new List<Explosion>();
    List<Explosion> bigExploList = new List<Explosion>();
    
    void Awake()
    {
        instance = this;
    }

    public Explosion PoolingExplosion(Transform t, int type)
    {
        switch (type)
        {
            case 0:
                for (int i = 0; i < smallExploList.Count; i++)
                {
                    if (!smallExploList[i].isActiveAndEnabled)
                    {
                        smallExploList[i].enabled = true;
                        return smallExploList[i];
                    }
                }

                GameObject small = Instantiate(smallExplosionPrefab, t.position, t.rotation) as GameObject;
                small.transform.parent = transform.parent;
                //newProj.gameObject.hideFlags = HideFlags.HideInHierarchy;
                Explosion s = small.GetComponent<Explosion>();
                smallExploList.Add(s);
                return s;
            case 1:
                for (int i = 0; i < mediumExploList.Count; i++)
                {
                    if (!mediumExploList[i].isActiveAndEnabled)
                    {
                        mediumExploList[i].enabled = true;
                        return mediumExploList[i];
                    }
                }

                GameObject med = Instantiate(mediumExplosionPrefab, t.position, t.rotation) as GameObject;
                med.transform.parent = transform.parent;
                //newProj.gameObject.hideFlags = HideFlags.HideInHierarchy;
                Explosion m = med.GetComponent<Explosion>();
                mediumExploList.Add(m);
                return m;
            case 2:
                for (int i = 0; i < bigExploList.Count; i++)
                {
                    if (!bigExploList[i].isActiveAndEnabled)
                    {
                        bigExploList[i].enabled = true;
                        return bigExploList[i];
                    }
                }

                GameObject large = Instantiate(bigExplosionPrefab, t.position, t.rotation) as GameObject;
                large.transform.parent = transform.parent;
                //newProj.gameObject.hideFlags = HideFlags.HideInHierarchy;
                Explosion l = large.GetComponent<Explosion>();
                bigExploList.Add(l);
                return l;
            default:
                return null;
        }
    }
}