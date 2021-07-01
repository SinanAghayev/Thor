using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    private Rigidbody myRigidbody;
    private Animator myAnimator;
    public GameObject kafaKamerasi;
    float kafaRotUstAlt = 0, kafaRotSagSol = 0;

    private float horizontal = 0, vertical = 0;

    private Vector3 moveDir = Vector3.zero;
    private Vector3 kameraArasiMesafe;

    RaycastHit hit;

    private bool aiming = false;
    public float throwPower = 20;
    private Rigidbody mjRb;
    public GameObject mj;

    public GameObject upLightning;
    public GameObject mjLightning;
    public GameObject fwLightning;
    private LineRenderer upLineRenderer;
    private LineRenderer mjLineRenderer;
    private LineRenderer fwLineRenderer;
    private Collider fwCollider;


    public GameObject eq;
    public GameObject hand;
    public GameObject eqInHand;
    public ParticleSystem particle;

    private bool canPull = false;
    private bool inHand = true;
    private bool canCast = false;

    Vector3 targetPosition;
    private Vector3 lookAtTarget;
    Quaternion playerRot;
    float rotSpeed = 5;
    float speed = 2;

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myAnimator = GetComponent<Animator>();
        kameraArasiMesafe = kafaKamerasi.transform.position - transform.position;

        mjRb = mj.GetComponent<Rigidbody>();

        upLineRenderer = upLightning.GetComponent<LineRenderer>();
        mjLineRenderer = mjLightning.GetComponent<LineRenderer>();
        fwLineRenderer = fwLightning.GetComponent<LineRenderer>();
        fwCollider = fwLightning.GetComponent<BoxCollider>();

        mjLineRenderer.enabled = false;
        upLineRenderer.enabled = false;
        fwLineRenderer.enabled = false;
        fwCollider.enabled = false;

        myAnimator.SetFloat("Speed", 1);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TemelHareketler();
        KameraKontrol();
        Aim();
        Pull();
        Attack();
        Lightning();

        if (Input.GetKeyDown(KeyCode.G) && !inHand)
        {
            Rotate();
            canPull = true;
        }





    }
    void TemelHareketler()
    {

        bool state = !(this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Throw 1") || 
                       this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Throw 2") || 
                       this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Pull") ||
                       this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack") ||
                       this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hand Up") ||
                       this.myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Hand Forward"));

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        Vector3 moveDir = new Vector3(horizontal, 0, vertical);
        moveDir = transform.TransformDirection(moveDir);

        if (Input.GetKey(KeyCode.LeftShift) && (horizontal != 0 || vertical != 0) && state)
        {
            myRigidbody.position += moveDir * Time.fixedDeltaTime * 6;
            myAnimator.SetFloat("Speed", 3);
        }
        else if(state)
        {
            myRigidbody.position += moveDir * Time.fixedDeltaTime * 3;
        }


        
        if(horizontal != 0 || vertical != 0 && !Input.GetKey(KeyCode.LeftShift) && state)
        {
            myAnimator.SetFloat("Speed", 2);
        }else if(!Input.GetKey(KeyCode.LeftShift))
        {
            myAnimator.SetFloat("Speed", 1);
        }
        
    }
    void KameraKontrol()
    {
        kafaKamerasi.transform.position = transform.position + kameraArasiMesafe;
        kafaRotUstAlt += Input.GetAxis("Mouse Y") * Time.fixedDeltaTime * -150;
        kafaRotSagSol += Input.GetAxis("Mouse X") * Time.fixedDeltaTime * 150;
        kafaRotUstAlt = Mathf.Clamp(kafaRotUstAlt,-30,30);
        kafaKamerasi.transform.rotation = Quaternion.Euler(kafaRotUstAlt, kafaRotSagSol, transform.eulerAngles.z);

        if (horizontal != 0 || vertical != 0)
        {
            Physics.Raycast(Vector3.zero,kafaKamerasi.transform.GetChild(0).forward,out hit);
            transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(new Vector3(hit.point.x,0,hit.point.z)),0.5f);
        }

        
    }

    void Aim()
    {

        aiming = Input.GetMouseButton(1);
        myAnimator.SetBool("aim", aiming && inHand);
            
        if(aiming && (Input.GetKeyDown(KeyCode.C) || Input.GetMouseButtonDown(0)))
        {
            myAnimator.SetTrigger("Throw");
        }
    }
    void Throw()
    {
        mjRb.isKinematic = false;
        mjRb.transform.parent = null;
        mjRb.AddForce(transform.forward * throwPower + transform.up * 4, ForceMode.Impulse);
        inHand = false;
    }

    void Rotate()
    {
        
        targetPosition = eq.transform.position;

        lookAtTarget = new Vector3(targetPosition.x - mj.transform.position.x, targetPosition.y - mj.transform.position.y, targetPosition.z - mj.transform.position.z);
        playerRot = Quaternion.LookRotation(lookAtTarget);

        mjRb.isKinematic = true;

        myAnimator.SetTrigger("Pull");

    }
    void Pull()
    {
        if (canPull)
        {
            mj.transform.rotation = Quaternion.Slerp(mj.transform.rotation, playerRot, rotSpeed * Time.deltaTime);
            if(mj.transform.position != targetPosition)
            {
                Invoke("SpeedIncreasing",0.5f);
                
            }
            mj.transform.position = Vector3.MoveTowards(mj.transform.position, targetPosition, rotSpeed * Time.deltaTime * speed);
        }
        if(mj.transform.position == targetPosition)
        {
            mj.transform.parent = hand.transform;
            myAnimator.SetTrigger("Pulled");
            canPull = false;
            inHand = true;
            speed = 1;

            mj.transform.position = eqInHand.transform.position;
            mj.transform.rotation = eqInHand.transform.rotation;
        }
    }
    void SpeedIncreasing()
    {
        speed *= speed * speed;
    }

    void Particle()
    {
        particle.Emit(1);
    }
    void Attack()
    {
        if (Input.GetKeyDown(KeyCode.Q) && inHand)
        {
            myAnimator.SetTrigger("Attack");
            Invoke("Particle", 1f);
        }
    }

    //Lightning
    void Lightning()
    {
        if (Input.GetKeyDown(KeyCode.E) && inHand)
        {
            myAnimator.SetTrigger("LightningUp");
            Invoke("LRE", 0.5f);
            Invoke("MRE", 0.5f);
            Invoke("LRD", 1f);
            Invoke("MRD", 10f);
            
        }
        
        if(Input.GetKeyDown(KeyCode.R) && canCast && inHand)
        {
            myAnimator.SetTrigger("LightningForward");
            Invoke("FRE", 0.30f);
            Invoke("FRD", 0.60F);
        }
    }
    void LRE()
    {
        upLineRenderer.enabled = true;
    }
    void LRD()
    {
        upLineRenderer.enabled = false;
    }
    void MRE()
    {
        mjLineRenderer.enabled = true;
        canCast = true;

    }
    void MRD()
    {
        mjLineRenderer.enabled = false;
        canCast = false;
    }
    void FRE()
    {
        fwLineRenderer.enabled = true;
        fwCollider.enabled = true;
        MRD();
    }
    void FRD()
    {
        fwLineRenderer.enabled = false;
        fwCollider.enabled = false;
    }
    //Lightning


}

