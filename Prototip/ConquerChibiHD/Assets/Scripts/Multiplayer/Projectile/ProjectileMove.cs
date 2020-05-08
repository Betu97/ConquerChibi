using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMove : MonoBehaviour
{
    public float speed;
    public float fireRate;
    public GameObject muzzlePrefab;
    public GameObject hitPrefab;
    private Vector3 currentTransform;

    // Start is called before the first frame update
    void Start(){
        currentTransform = transform.forward;
        Debug.Log("currentTransform pre minus: " + currentTransform);
        currentTransform.y = currentTransform.y - (float) 0.17;
        Debug.Log("currentTransform post minus: " + currentTransform);
        if (muzzlePrefab != null){
            var muzzleVFX = Instantiate(muzzlePrefab, transform.position, Quaternion.identity);
            muzzleVFX.transform.forward = gameObject.transform.forward;
            var psMuzzle = muzzleVFX.GetComponent<ParticleSystem>();
            if(psMuzzle != null)
            {
                Destroy(muzzleVFX, psMuzzle.main.duration);
            }
            else
            {
                var psChild = muzzleVFX.transform.GetChild (0).GetComponent<ParticleSystem>();
                Destroy(muzzleVFX, psChild.main.duration);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(transform.position, Camera.main.transform.forward * 10, Color.green);
        if(speed != 0)
        {
            transform.position += (currentTransform + new Vector3(0, 0.3f, 0)) * (speed * Time.deltaTime);
        }
        else
        {
            Debug.Log("no Speed");
        }
    }

    void OnCollisionEnter(Collision col)
    {
        speed = 0;

        ContactPoint contact = col.contacts[0];
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
        Vector3 pos = contact.point;

        if(hitPrefab != null)
        {
            var hitVFX = Instantiate(hitPrefab, pos, rot);
            var psHit = hitVFX.GetComponent<ParticleSystem>();
            if (psHit != null)
            {
                Destroy(hitVFX, psHit.main.duration);
            }
            else
            {
                var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
                Destroy(hitVFX, psChild.main.duration);
            }
        }

        Destroy(gameObject);
        
    }
}