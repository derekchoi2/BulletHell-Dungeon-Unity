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

	private List<GameObject> projectiles;
	private float nextShot;
	private float shotTimer = 0f;
	private Vector3 startPos = new Vector3(0,1,0);

	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		nextShot = timeBetweenShots;
		gc = GameController.Instance;
		rb = GetComponent<Rigidbody> ();
		projectiles = new List<GameObject> ();
		Reset ();
	}

	// Update is called once per frame
	void Update () {

		shotTimer = shotTimer + Time.deltaTime;

		if ((Input.GetAxis("ShootVertical") != 0 || Input.GetAxis("ShootHorizontal") != 0) && shotTimer > nextShot)
		{
			nextShot = shotTimer + timeBetweenShots;
			FireProjectile ();
			nextShot = nextShot - shotTimer;
			shotTimer = 0.0F;
		}
	}

	void FireProjectile(){
		GameObject projectileInstance = Instantiate (projectile, transform.position, transform.rotation);
		BasicProjectile projectileScript = projectileInstance.GetComponent<BasicProjectile> ();
		projectileScript.owner = BasicProjectile.Owner.Player;

		float shootHorizontal = Input.GetAxis ("ShootHorizontal");
		float shootVertical = Input.GetAxis ("ShootVertical");
		Vector3 dir = new Vector3 (shootHorizontal, 0, shootVertical);

		projectileScript.SetVelocity (dir.normalized * PlayerController.Instance.BasicProjectileSpeed * Time.deltaTime);

		projectiles.Add (projectileInstance);
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
			CollectPickup (collider.gameObject.GetComponent<Pickup>());
		}

		if ((collider.gameObject.CompareTag ("Projectile") && collider.gameObject.GetComponent<BasicProjectile>().owner == BasicProjectile.Owner.Enemy) ||
			collider.gameObject.CompareTag("Enemy")) {
			//player collide with enemy projectile OR enemy
			gc.Reset();
			Destroy (gameObject);
		}
	}

	void CollectPickup(Pickup pickup){
		switch (pickup.type) {
		case PickupTypes.firerate:
			timeBetweenShots += pickup.value;
			break;
		}
		PickupController.Instance.PickupCollected ();
	}

	public void Reset(){
		foreach (GameObject projectile in projectiles) {
			Destroy (projectile);
		}
		projectiles.Clear ();
		ResetPos ();
		score = 0;
	}

	public void ResetPos(){
		transform.position = startPos;
	}


}
