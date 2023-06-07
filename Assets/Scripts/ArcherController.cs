using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArcherController : MonoBehaviour
{
    private Animator _animator;
    private CharacterController _characterController;

    private float rotation = 0.0f;
    private float rotationY = 0.0f;

    private int velocityXHash;
    private int velocityZHash;
    private int isShootingHash;
    private int isDeadHash;
    public float acceleration = 0.8f;
    public float speed = 1f;
    private float velocityX = 0.0f;
    private float velocityZ = 0.0f;

    private bool isShooting;
    private ArrowController currentArrow;
    private GameObject currentTarget;
    public Transform arrowPos;
    public GameObject arrowPrefab;

    public int health = 100;
    public bool isDead = false;
    public GameController gameController;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _characterController = GetComponent<CharacterController>();
        velocityXHash = Animator.StringToHash("Velocity X");
        velocityZHash = Animator.StringToHash("Velocity Z");
        isShootingHash = Animator.StringToHash("isShooting");
        isDeadHash = Animator.StringToHash("dead");
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            MoveCharacter();
            CalculateRotationFromInput();
            CheckShooting();
        }
    }


    private void CheckShooting()
    {
        if (!isShooting)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    GameObject hitObject = hit.collider.gameObject;
                    if (hitObject.CompareTag("Enemy"))
                    {
                        StartShooting(hitObject);
                    }
                }
            }
        }

        if (isShooting && currentTarget != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation((currentTarget.transform.position - transform.position).normalized);
            targetRotation.z = 0;
            float sharpness = 0.1f;
            float blend = 1f - Mathf.Pow(1f - sharpness, 30f * Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, blend); 
        }
    }

    public void StartShooting(GameObject target)
    {
        isShooting = true;
        velocityZ = 0;
        velocityX = 0;
        _animator.SetFloat(velocityXHash, velocityX);
        _animator.SetFloat(velocityZHash, velocityZ);
        // transform.LookAt(target.transform);
        currentTarget = target;
        _animator.SetBool(isShootingHash, isShooting);
        
    }

    public void ShootingFinished()
    {
        isShooting = false;
        _animator.SetBool(isShootingHash, isShooting);
        currentTarget = null;
    }

    public void FireArrow()
    {
        currentArrow = Instantiate(arrowPrefab, arrowPos.position, transform.rotation).GetComponent<ArrowController>();
        // arrow.transform.localPosition = arrowPos.localPosition;
        Transform targetPos = null;
        if (currentTarget.GetComponent<ZombieController>())
        {
            targetPos = currentTarget.transform.Find("TargetPosition");
        }
        else
        {
            targetPos = currentTarget.transform;
        }
        
        currentArrow.Target = targetPos;
    }

    private void CalculateRotationFromInput()
    {
        if (!isShooting)
        {
            float sensitivity = 2f;
            if(Input.GetKey("a"))
                transform.Rotate(0, -sensitivity, 0);
            if(Input.GetKey("d"))
                transform.Rotate(0, sensitivity, 0);
            
            // rotation += Input.GetAxis("Mouse X") * Time.deltaTime * 100f; // * -Time.deltaTime;
            // rotationY += Input.GetAxis("Mouse Y") * -Time.deltaTime;
            // // rotation = Mathf.Clamp(rotation, 0, 360);
            // transform.localRotation = Quaternion.AngleAxis(rotation, Vector3.up);
        }
    }

    private void MoveCharacter()
    {
        if (isShooting)
        {
            return;
        }

        bool wPressed = Input.GetKey("w");
        bool aPressed = Input.GetKey("a");
        bool dPressed = Input.GetKey("d");
        bool sPressed = Input.GetKey("s");

        if (wPressed && velocityZ < 2f)
        {
            velocityZ += Time.deltaTime * acceleration;
        }

        if (!wPressed && velocityZ > 0.0f)
        {
            velocityZ -= Time.deltaTime * acceleration;
        }

        if (sPressed && !dPressed && !aPressed && velocityZ > -0.5f)
        {
            velocityZ -= Time.deltaTime * acceleration;
        }

        if (!wPressed && !sPressed)
        {
            if (velocityZ < 0)
            {
                velocityZ += Time.deltaTime * acceleration;
            }
        }

        if (aPressed && velocityX > -2f)
        {
            velocityX -= Time.deltaTime * acceleration;
        }

        if (dPressed && velocityX < 2f)
        {
            velocityX += Time.deltaTime * acceleration;
        }

        if (!aPressed && velocityX < 0.0f)
        {
            velocityX += Time.deltaTime * acceleration;
        }

        if (!dPressed && velocityX > 0.0f)
        {
            velocityX -= Time.deltaTime * acceleration;
        }

        Vector3 moveVector = transform.right * velocityX + transform.forward * velocityZ;
        _characterController.Move(moveVector * speed * Time.deltaTime);
        _animator.SetFloat(velocityXHash, velocityX);
        _animator.SetFloat(velocityZHash, velocityZ);
    }

    public void GetDamage(int damage)
    {
        health -= damage;
        gameController.HealthChanged(health);

        if (health <= 0)
        {
            isDead = true;
            _animator.SetBool(isDeadHash, true);
        }
    }
}