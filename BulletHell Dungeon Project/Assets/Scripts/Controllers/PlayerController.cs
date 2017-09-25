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

	public GameObject leftJoystickObject;
	public GameObject rightJoystickObject;

	private VirtualJoystick leftJoystick;
	private VirtualJoystick rightJoystick;

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


		leftJoystick = leftJoystickObject.GetComponent<VirtualJoystick> ();
		rightJoystick = rightJoystickObject.GetComponent<VirtualJoystick> ();

		#if UNITY_IOS || UNITY_ANDROID
		#elif UNITY_STANDALONE || UNITY_EDITOR
		leftJoystickObject.GetComponent<Image>().enabled = false;
		leftJoystickObject.transform.GetChild (0).GetComponent<Image> ().enabled = false;
		rightJoystickObject.GetComponent<Image>().enabled = false;
		rightJoystickObject.transform.GetChild (0).GetComponent<Image> ().enabled = false;
		leftJoystick.enabled = false;
		rightJoystick.enabled = false;
		#endif
	}

	// Update is called once per frame
	void Update () {
		if (!dead) {
			Move ();

			shotTimer = shotTimer + Time.deltaTime;

			if (shotTimer > nextShot) {	
				if (state == States.Attack) {
					state = States.Idle;
					animator.ChangeState (state, direction);
				}

				#if UNITY_STANDALONE || UNITY_EDITOR
				float shootVertical = Input.GetAxis ("ShootVertical");
				float shootHorizontal = Input.GetAxis ("ShootHorizontal");
				#elif UNITY_IOS || UNITY_ANDROID
				float shootHorizontal = rightJoystick.GetDiscrete().x;
				float shootVertical = rightJoystick.GetDiscrete().z;
				#endif

				if (shootVertical != 0 || shootHorizontal != 0) {
					nextShot = shotTimer + timeBetweenShots;
					FireProjectile (shootHorizontal, shootVertical);
					nextShot = nextShot - shotTimer;
					shotTimer = 0.0F;
				}
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
		projectileScript.SetVelocity (dir * PlayerController.Instance.BasicProjectileSpeed * Time.fixedDeltaTime);

		projectiles.Add (projectileInstance);
	}

	void Move(){
		#if UNITY_STANDALONE || UNITY_EDITOR
		float moveHorizontal = Input.GetAxisRaw ("Horizontal");
		float moveVertical = Input.GetAxisRaw ("Vertical");
		#elif UNITY_IOS || UNITY_ANDROID
		float moveHorizontal = leftJoystick.GetDiscrete().x;
		float moveVertical = leftJoystick.GetDiscrete().z;
		#endif

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
			if (z < 0.5 && z > -0.5) {
				return Directions.W;
			} else if (z >= 0.5) {
				return Directions.NW;
			} else if (z <= -0.5) {
				return Directions.SW;
			}
		} else if (x > 0) { //right
			if (z < 0.5 && z > -0.5) {
				return Directions.E;
			} else if (z >= 0.5) {
				return Directions.NE;
			} else if (z <= -0.5) {
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
