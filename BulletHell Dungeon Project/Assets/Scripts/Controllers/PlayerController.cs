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

	public int health = 3;
	private int maxHealth;

	public GameObject BaseWeapon;
	[HideInInspector] public GameObject CurrentWeapon;
	private Weapon CurrentWeaponScript;
	public GameObject WeaponSpawnOrigin;

	public float speed = 20f;
	public float score = 0f;
	private bool dead = true;

	public States state;
	public Directions direction;
	private SpriteAnimator animator;
	private SpriteRenderer spriteRenderer;
	private Vector3 startPos = new Vector3(0,1,0);

	private Rigidbody rb;

	private List<GameObject> sentries = new List<GameObject> ();

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
		gc = GameController.Instance;
		rb = GetComponent<Rigidbody> ();

		maxHealth = health;
		Reset ();
		Hide ();
	}

	// Update is called once per frame
	void Update () {
		if (!dead) {
			Vector3 moveVec = Vector3.zero, shootVec = Vector3.zero;
			if (!enableJoysticks) {
				shootVec.x = Input.GetAxis ("ShootHorizontal");
				shootVec.z = Input.GetAxis ("ShootVertical");
				moveVec.x = Input.GetAxisRaw ("Horizontal");
				moveVec.z = Input.GetAxisRaw ("Vertical");
			} else {
				shootVec.x = rightStickVec.x;
				shootVec.z = rightStickVec.y;
				moveVec.x = leftStickVec.x;
				moveVec.z = leftStickVec.y;
			}

			Move (moveVec);
			CurrentWeaponScript.Shoot (shootVec);

			if (sentries.Count > 0)
				SpreadSentries ();
		}
	}

	void Move(Vector3 moveVec){

		Vector3 dir = moveVec.normalized;

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

		if (moveVec.sqrMagnitude != 0) {
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

	void OnTriggerEnter(Collider collider){
		if (!dead) {
			if (collider.gameObject.CompareTag ("Door")) {
				//collide with player
				PlayerDoorCollide (transform.position);
				LevelController.Instance.PlayerDoorCollide ();
			}

			if (collider.gameObject.CompareTag ("Pickup")) {
				CollectPickup (collider.gameObject.GetComponent<Pickup> ());
			}

			if ((collider.gameObject.CompareTag ("Projectile") && collider.gameObject.GetComponent<BasicProjectile> ().owner == BasicProjectile.Owner.Enemy) ||
			   collider.gameObject.CompareTag ("Enemy")) {
				//player collide with enemy projectile OR enemy
				PlayerHit(collider.gameObject);
			}
		}
	}

	void PlayerHit(GameObject collider){
		int damage;
		//damage according to current enemy health or damage value of projectile depending on type
		if (collider.CompareTag ("Enemy"))
			damage = collider.GetComponentInChildren<HealthBar> ().health;
		else
			damage = collider.GetComponent<BasicProjectile> ().Damage;
		
		health = Mathf.Clamp (health - damage, 0, maxHealth);

		if (health <= 0) {
			CurrentWeaponScript.Reset ();
			CurrentWeapon = BaseWeapon;
			DestroySentries ();
			Hide ();
			gc.PlayerDie ();
		}
	}

	public void SwapWeapon(GameObject NewWeapon){
		if (CurrentWeapon != NewWeapon) {
			Destroy (CurrentWeapon);
			CurrentWeapon = Instantiate (NewWeapon, WeaponSpawnOrigin.transform);
			CurrentWeaponScript = CurrentWeapon.GetComponent<Weapon> ();
		}
	}

	public void Hide(){
		spriteRenderer.enabled = false;
		if (CurrentWeaponScript != null)
			CurrentWeaponScript.Hide ();
		dead = true;
	}

	public void Show(){
		spriteRenderer.enabled = true;
		SwapWeapon (BaseWeapon);
		if (dead)
			Reset ();
		dead = false;
	}

	public void EnemyKilled(){
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

		//position sentries on player, allow them to spread naturally
		foreach (GameObject sentry in sentries)
			sentry.transform.position = rb.position;
	}

	void CollectPickup(Pickup pickup){
		switch (pickup.type) {
		case PickupTypes.firerateUp:
			CurrentWeaponScript.timeBetweenShots += pickup.value;
			break;
		case PickupTypes.sentry:
			sentries.Add(Instantiate(pickup.SpawnPrefab, transform.position, Quaternion.identity));
			break;
		}
	}

	public void Reset(){
		ResetPos ();
		DestroySentries ();
		state = States.Idle;
		direction = Directions.Unspecified;
		animator.ChangeState (state, direction);
		score = 0;
		health = maxHealth;
		if (CurrentWeaponScript != null)
			CurrentWeaponScript.Reset ();
	}

	public void ChangeState(States state, Vector3 dirVec){
		this.state = state;
		direction = CalculateDirection (dirVec);
		animator.ChangeState (state, direction);
	}

	void SpreadSentries(){
		for (int i = 0; i < sentries.Count; i++) {
			for (int j = 0; j < sentries.Count; j++) {
				if (i != j) { //don't compare same indexes
					float distance = Vector3.Distance (sentries [i].transform.position, sentries [j].transform.position);
					if (distance < 2) {
						if (distance == 0) {
							//if distance 0, won't go anywhere. nudge i's x by 0.5
							Vector3 tempPos = sentries [i].transform.position;
							tempPos.x += 0.5f; 
							sentries [i].transform.position = tempPos;
						} else {
							sentries [i].transform.position += (sentries [i].transform.position - sentries [j].transform.position).normalized * Time.deltaTime;
							sentries [j].transform.position += (sentries [j].transform.position - sentries [i].transform.position).normalized * Time.deltaTime;
						}
					}
				}
			}
		}
	}

	void DestroySentries(){
		foreach (GameObject sentry in sentries) {
			Destroy (sentry);
		}
		sentries.Clear ();
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
