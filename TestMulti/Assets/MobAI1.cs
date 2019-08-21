using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;

public class MobAI1 : MonoBehaviour
{
    [SerializeField] private GameObject _head;
    [SerializeField] Actions myState;
    [SerializeField] private int _detectionDistance;
    private int _layer_mask;
    private Rigidbody _body;
    private Animator _headAnimator;
    private bool _stun;
    [SerializeField] private NavMeshAgent _navMeshAgent;

    [SerializeField] private GameObject _target;

    // Start is called before the first frame update
    void Start()
    {
        _body = gameObject.GetComponent<Rigidbody>();
        _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();   
        _layer_mask = LayerMask.GetMask("Player");
        _headAnimator = _head.GetComponent<Animator>();

        myState = Actions.wait;
    }
    public enum Actions
    {
        wait,
        patrol,
        stun,
        follow       
    }
    // Update is called once per frame
    void Update()
    {
        switch (myState)
        {
            case Actions.wait:
                // Debug.Log("wait ongoing !");
                _headAnimator.Play("jaune");
                CheckActions();
                break;
            case Actions.patrol:
                // Debug.Log("Patrol ongoing !");
                _headAnimator.Play("jaune");
                CheckActions();
                Patrol();
                break;
            case Actions.stun:
                //Debug.Log("stun incoming !");
                _headAnimator.Play("Gris");
                Stun();
                break;
            case Actions.follow:
                //Debug.Log("follow incoming !");
                _headAnimator.Play("rouge");
                Follow();
                break;
           
        }
    }
    public void CheckActions()
    {
        Debug.DrawRay(transform.transform.position,transform.forward, Color.green); 

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _detectionDistance, _layer_mask))
        {
            // Debug.Log("found ya boi");
            _target = hit.collider.gameObject;
            myState = Actions.follow;
        }
        

     /* RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, transform.right * side, 200f, layer_mask);
        Debug.DrawLine(transform.position, hit.point, new Color(252, 252, 0));*/   
    }
    public void Follow()
    {
        if (_target != null)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.destination = _target.transform.position;
        }
        else
        {
            myState = Actions.wait;
        }
    }
    public void Patrol()
    {

    }
    public void Stun()
    {
        if (_stun == false)
        {
            _stun = true;
            StopAllCoroutines();
            StartCoroutine("stunWait");
        }
    }
    void OnCollisionEnter(Collision collision)
    {
       if (collision.gameObject.tag == "Pickable")
        {
            myState = Actions.stun;
            _target = null;
            _navMeshAgent.isStopped = true;
        }
    }
    IEnumerator stunWait()
    {
        yield return new WaitForSeconds(3.0f);
        _stun = false;
        _body.velocity = Vector3.zero;
        _body.angularVelocity = Vector3.zero;
        myState = Actions.wait;
    }
}
