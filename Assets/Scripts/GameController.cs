using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public ArcherController archer;

    public GameObject zombiePrefab;
    private List<ZombieController> zombies = new List<ZombieController>();
    public Text healthText;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SpawnZombie();
        // InvokeRepeating ("SpawnZombie", 3, 3);
    }

    void SpawnZombie()
    {
        if(zombies.Count > 100)
            return;
        
        int range = 70;
        int spawnPointX = Random.Range(-range, range);
        int spawnPointZ = Random.Range(-range, range);
        Vector3 spawnPosition = new Vector3(archer.transform.position.x  + spawnPointX, 500, archer.transform.position.z + spawnPointZ);
        if (Physics.Raycast(spawnPosition, Vector3.down, out RaycastHit hit))
        {
            spawnPosition = hit.point;
            // Debug.Log("hit.point " + hit.point);
            ZombieController zombie = Instantiate (zombiePrefab, spawnPosition, Quaternion.Euler(0, Random.Range(0, 360), 0)).GetComponent<ZombieController>();
            zombie.archer = archer;
        
            zombies.Add(zombie);
        }

    }

    public void HealthChanged(int health)
    {
        Debug.Log("Health=" + health);
        healthText.text = "Health: " + health;
    }
}
