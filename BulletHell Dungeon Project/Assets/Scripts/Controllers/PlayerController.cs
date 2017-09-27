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

	public InnerJoystick leftJoystick;
	public InnerJoystick rightJoystick;

	public Vector3 leftStickVec { get; set; }
	public Vector3 rightStickVec { get; set; }

	private GameController gc;

	public GameObject projectile;
	public float timeBetweenShots = 0.5f;
	public float BasicProjectileSpeed = 40f;
	public float speed = 20f;
	public float score = 0f;
	private bool dead = false;

	private States state;
	private Directions direction;
	private SpriteAnimator animator;
	private SpriteRenderer spriteRenderer;

	private float savedTimeBetweenShots;

	private List<GameObject> projectiles;
	private float nextShot;
	private float shotTimer = 0f;
	private Vector3 startPos = new Vector3(0,1,0);

	private Rigidbody rb;

	#if UNITY_IOS || UNITY_ANDROID
	private bool enableJoysticks = true;
	#else
	private bool enableJoysticks = false;
	#endif

	// Use this for initialization
	void Start () {
		state = States.Idle;
		direction = Directions.Unspecified;
		animator = gameObject.GetComponentInChildren<SpriteAnimator> ();
		spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer> ();
		nextShot = timeBetweenShots;
		savedTimeBetweenShots = timeBetweenShots;
		gc = GameController.Instance;
		rb = GetComponent<Rigidbody> ();
		projectiles = new List<GameObject> ();
		Reset ();
		Hide ();
	}

	// Update is called once per frame
	void Update () {
		if (!dead) {
			Move ();
			Shoot ();
		}
	}

	void Shoot(){
		shotTimer = shotTimer + Time.deltaTime;

		if (shotTimer > nextShot) {	
			if (state == States.Attack) {
				state = States.Idle;
				animator.ChangeState (state, direction);
			}

			float shootVertical, shootHorizontal;
			if (!enableJoysticks) {
				shootVertical = Input.GetAxis ("ShootVertical");
				shootHorizontal = Input.GetAxis ("ShootHorizontal");
			} else {
				shootHorizontal = rightStickVec.x;
				shootVertical = rightStickVec.y;
			}

			if (shootVertical != 0 || shootHorizontal != 0) {
				nextShot = shotTimer + timeBetweenShots;
				FireProjectile (shootHorizontal, shootVertical);
				nextShot = nextShot - shotTimer;
				shotTimer = 0.0F;
			}
		}
	}

	void FireProjectile(float shootHorizontal, float shootVertical){
		GameObject projectileInstance = Instantiate (projectile, transform.position, transform.rotation);
		BasicProjectile projectileScript = projectileInstance.GetComponent<BasicProjectile> ();
		projectileScript.owner = BasicProjectile.Owner.Player;

		Vector3 dir = new Vector3 (shootHorizontal, 0, shootVertical).normalized;

		Directions attackdir = CalculateDirection (dir);

		state = States.Attack;
		animator.ChangeState (state, attackdir);
		projectileScript.SetVelocity (dir * BasicProjectileSpeed * Time.fixedDeltaTime);

		projectiles.Add (projectileInstance);
	}

	void Move(){
		float moveHorizontal, moveVertical;
		if (!enableJoysticks) {
			moveHorizontal = Input.GetAxisRaw ("Horizontal");
			moveVertical = Input.GetAxisRaw ("Vertical");
		} else {
			moveHorizontal = leftStickVec.x;
			moveVertical = leftStickVec.y;
		}

		Vector3 dir = new Vector3 (moveHorizontal, 0, moveVertical).normalized;

		rb.velocity = dir * speed;

		rb.position = new Vector3 (
			Mathf.Clamp (rb.position.x, GameController.Instance.boundary.xMin, GameController.Instance.boundary.xMax),
			0f,
			Mathf.Clamp (rb.position.z, GameController.Instance.boundary.zMin, GameController.Instance.boundary.zMax));

		Directions newDir = CalculateDirection (dir);
		//smooth out directional movement animations
		if (newDir == Directions.NE || newDir == Directions.SE)
			newDir = Directions.E;
		if (newDir == Directions.NW || newDir == Directions.SW)
			newDir = Directions.W;

		if (moveHorizontal != 0 || moveVertical != 0) {
			if (state == States.Idle) {
				state = States.Move;
				direction = newDir;
				animator.ChangeState (state, direction);
			} else if (direction != newDir) {
				direction = newDir;
				animator.ChangeState (state, direction);
			}
		} else {
			if (state != States.Idle && state != States.Attack) {
				state = States.Idle;
				direction = Directions.Unspecified;
				animator.ChangeState (state, direction);
			}
		}

	}

	//if hit door, reset position
	void OnTriggerEnter(Collider collider){
		if (!dead) {
			if (collider.gameObject.CompareTag ("Door")) {
				//collide with player
				PlayerDoorCollide (transform.position);
				LevelController.Instance.PlayerDoorCollide ();
			}

			if (collider.gameObject.CompareTag ("Pickup")) {
				Debug.Log ("Player detect pickup");
				CollectPickup (collider.gameObject.GetComponent<Pickup> ());
			}

			if ((collider.gameObject.CompareTag ("Projectile") && collider.gameObject.GetComponent<BasicProjectile> ().owner == BasicProjectile.Owner.Enemy) ||
			   collider.gameObject.CompareTag ("Enemy")) {
				//player collide with enemy projectile OR enemy
				DestroyProjectiles ();
				Hide ();
				gc.PlayerDie ();
			}
		}
	}

	public void Hide(){
		spriteRenderer.enabled = false;
		dead = true;
	}

	public void Show(){
		spriteRenderer.enabled = true;
		if (dead) {
			Reset ();
		}
		dead = false;
	}

	public void EnemyHit(){
		score += ((gc.level - 1) * 3) + gc.sublevel;
	}

	public void PlayerDoorCollide(Vector3 position){
		//appear as if went through door
		Vector3 nextPos;
		float x = position.x;
		float y = position.y;
		float z = position.z;
		if (z > gc.boundary.zMax - 5)
			nextPos = new Vector3 (x, y, z * -1);
		else if (z < gc.boundary.zMin + 5)
			nextPos = new Vector3 (x, y, z * -1);
		else if (x > gc.boundary.xMax - 5)
			nextPos = new Vector3 (x * -1, y, z);
		else
			nextPos = new Vector3 (x * -1, y, z);
		rb.position = nextPos;

	}

	void CollectPickup(Pickup pickup){
		switch (pickup.type) {
		case PickupTypes.firerate:
			timeBetweenShots += pickup.value;
			break;
		}
		PickupController.Instance.PickupCollected ();
	}

	void DestroyProjectiles(){
		foreach (GameObject projectile in projectiles) {
			Destroy (projectile);
		}
		projectiles.Clear ();
	}

	public void Reset(){
		DestroyProjectiles ();
		ResetPos ();
		state = States.Idle;
		direction = Directions.Unspecified;
		animator.ChangeState (state, direction);
		score = 0;
		timeBetweenShots = savedTimeBetweenShots;
	}

	public void ResetPos(){
		transform.position = startPos;
	}

	protected Directions CalculateDirection(Vector3 vec){
		float x = vec.x;
		float z = vec.z;

		if (x < 0) { //left
			if (z < 0.3 && z > -0.3) {
				return Directions.W;
			} else if (z >= 0.3) {
				return Directions.NW;
			} else if (z <= -0.3) {
				return Directions.SW;
			}
		} else if (x > 0) { //right
			if (z < 0.3 && z > -0.3) {
				return Directions.E;
			} else if (z >= 0.3) {
				return Directions.NE;
			} else if (z <= -0.3) {
				return Directions.SE;
			}
		} else if (z > 0) { //up
			return Directions.N;
		} else if (z < 0) { //down
			return Directions.S;
		}
		//default
		return Directions.Unspecified;
		
	}
}
