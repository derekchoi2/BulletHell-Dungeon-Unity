using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public static PlayerController Instance = null;
	void Awake(){
		if (Instance == null) Instance = this;
		else if (Instance != this) Destroy(gameObject);

		DontDestroyOnLoad(gameObject);
	}

	private GameController gc;

	public GameObject projectile;
	public float timeBetweenShots = 0.5f;
	public float BasicProjectileSpeed = 40f;
	public float speed = 20f;
	public float score = 0f;
	public Text timeBetweenShotsText;

	private float nextShot;
	private float shotTimer = 0f;
	private Vector3 startPos = new Vector3(0,1,0);

	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		nextShot = timeBetweenShots;
		gc = GameController.Instance;
		rb = GetComponent<Rigidbody> ();
		Reset ();
	}

	// Update is called once per frame
	void Update () {

		shotTimer = shotTimer + Time.deltaTime;

		if ((Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)) && shotTimer > nextShot)
		{
			nextShot = shotTimer + timeBetweenShots;
			FireProjectile ();
			nextShot = nextShot - shotTimer;
			shotTimer = 0.0F;
		}
	}

	void FireProjectile(){
		BasicProjectile shot = Instantiate (projectile, transform.position, transform.rotation).GetComponent<BasicProjectile> ();
		shot.owner = BasicProjectile.Owner.Player;

		//determine direction
		float x = 0;
		float z = 0;
		if (Input.GetKey (KeyCode.UpArrow))
			z += 1;
		else if (Input.GetKey (KeyCode.DownArrow))
			z -= 1;
		if (Input.GetKey (KeyCode.LeftArrow))
			x -= 1;
		else if (Input.GetKey (KeyCode.RightArrow))
			x += 1;
		Vector3 dir = new Vector3 (x, 0, z);
		shot.SetVelocity (dir.normalized * PlayerController.Instance.BasicProjectileSpeed * Time.deltaTime);
	}

	void FixedUpdate(){
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3 (moveHorizontal, 0, moveVertical);
		rb.velocity = movement * speed;

		rb.position = new Vector3 (
			Mathf.Clamp (rb.position.x, GameController.Instance.boundary.xMin, GameController.Instance.boundary.xMax),
			0f,
			Mathf.Clamp (rb.position.z, GameController.Instance.boundary.zMin, GameController.Instance.boundary.zMax));
	}

	//if hit door, reset position
	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.CompareTag("Door")){
			//collide with player
			gc.ClearRoom();
		}

		if (collider.gameObject.CompareTag ("Pickup")) {
			Debug.Log ("Player detect pickup");
			CollectPickup (collider.gameObject, collider.gameObject.GetComponent<Pickup>());
			GameController.Instance.StartPickupTimer ();
		}

		if ((collider.gameObject.CompareTag ("Projectile") && collider.gameObject.GetComponent<BasicProjectile>().owner == BasicProjectile.Owner.Enemy) ||
			collider.gameObject.CompareTag("Enemy")) {
			//player collide with enemy projectile OR enemy
			gc.Reset();
			Destroy (gameObject);
			Destroy (collider.gameObject); //destroy projectile
		}
	}

	void CollectPickup(GameObject gameObject, Pickup pickup){
		switch (pickup.type) {
		case PickupTypes.firerate:
			timeBetweenShots += pickup.value;
			break;
		}
	}

	public void Reset(){
		ResetPos ();
		score = 0;
	}

	public void ResetPos(){
		transform.position = startPos;
	}


}
