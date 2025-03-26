using UnityEngine;
using System.Collections;

public class TargetBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet") {
            StartCoroutine(waiter());
        }
    }

    IEnumerator waiter()
    {
        gameObject.GetComponent<Renderer>().material.color = Color.green;
        yield return new WaitForSeconds(1);
        gameObject.GetComponent<Renderer>().material.color = Color.red;
    }
}
