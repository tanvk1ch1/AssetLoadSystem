using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectS
{
    public class CutManager : MonoBehaviour
    {
        //[SerializeField] GameObject victim = default; //カットするオブジェクト
        Vector3 anchorPoint; // カットする場所
        Vector3 normalDirection; // カットする向き
        public Material capMaterial;
        
        private void Update()
        {
            // if (Input.GetKeyDown(KeyCode.Space))
            // {
                anchorPoint = transform.position + new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                normalDirection = transform.position + new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f));
                
                GameObject[] cutObjs = MeshCut.Cut(gameObject, anchorPoint, normalDirection, capMaterial);
                cutObjs[0].AddComponent<Rigidbody>();
                Destroy(cutObjs[0].GetComponent<BoxCollider>());
                cutObjs[0].AddComponent<BoxCollider>();
                
                cutObjs[1].AddComponent<Rigidbody>();
                cutObjs[1].AddComponent<BoxCollider>();
                // cutObjs[1].AddComponent<ProjectS>().capMaterial = capMaterial;
            // }
        }
    }
}