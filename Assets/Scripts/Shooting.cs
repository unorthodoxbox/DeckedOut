using UnityEngine;

public class Shooting : MonoBehaviour
{
    public GameObject gun;
    public GameObject bullet;

    public GameObject camera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            //Debug.Log("Shot!");
            Vector3 gunPos = new Vector3(gun.transform.position.x, gun.transform.position.y, gun.transform.position.z);
            Quaternion gunRot = new Quaternion(camera.transform.rotation.x, camera.transform.rotation.y, camera.transform.rotation.z * -1, 1);
            //temp3.
            Instantiate(bullet, gunPos, gunRot);

            //
        }
    }
}
