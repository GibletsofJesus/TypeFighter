﻿using UnityEngine;
using System.Collections;

public class Player : Actor 
{
	private static Player staticInstance = null;
	public static Player instance {get {return staticInstance;} set{}}

    [SerializeField] private SpriteRenderer spriteRenderer = null;
    [SerializeField] private SpriteRenderer laserRenderer = null;

	protected float updatedDefaultHealth;
	[SerializeField] private int defaultLives = 3;
	private int lives;
    public int numLives { get { return lives; } }

    public int score = 0;
    public bool advertAttack = false;
    //public float moveSpeed;
    public AudioClip[] shootSounds;
    public ParticleSystem[] muzzleflash;
    Vector3 rotLerp;
    //public GameObject target;
    Vector3 screenBottom;
    Vector2 verticalBoundsBot, verticalBoundsTop;

    private bool isInvincible = false;
    private float invincibleFlickerRate = 0.25f;
    private float invincibleFlickerCooldown = 0.0f;
    private bool flickerDown = true;
    private int addAmount = 0;
    private int randomAdAmount;
    private float adCool = 0;
    private float maxAdCool;

    private bool backupAvailable = false;
    [SerializeField] private PlayerBackup[] backups = null;

    [SerializeField] private float respawnInvincibility = 0.0f;
    private float respawnTime = 0.0f;

    [SerializeField] private GameObject[] childObjects = null;

    protected override void Awake()
	{
        maxAdCool = Random.Range(1, 3);
        randomAdAmount = Random.Range(2, 5);
		staticInstance = this;
		base.Awake ();
		updatedDefaultHealth = defaultHealth;
		lives = defaultLives;
        screenBottom = Camera.main.ViewportToWorldPoint(new Vector3(.3f, -.5f, .3f));
        verticalBoundsBot = Camera.main.ViewportToWorldPoint(new Vector3(.5f, 0f));
        verticalBoundsTop = Camera.main.ViewportToWorldPoint(new Vector3(.5f, .5f));
    }
    void Start()
    {
        if (GreenShip.instance)
        {
            spriteRenderer.sprite = GreenShip.instance.ship;
            //Destroy(GreenShip.instance.gameObject);
        }
    }
	// Update is called once per frame
    protected void FixedUpdate()
    {
        if (GameStateManager.instance.state == GameStateManager.GameState.Gameplay)
        {
            base.Update();

            if (transform.position.x <= -screenBottom.x - 1f)
            {
                rig.AddForce(Vector2.right * 2, ForceMode2D.Impulse);
            }
            if (transform.position.x >= screenBottom.x + 1f)
            {
                rig.AddForce(-Vector2.right * 2, ForceMode2D.Impulse);
            }
            if (transform.position.y >= verticalBoundsTop.y / 2 - 1f)
            {
                rig.AddForce(-Vector2.up * 2, ForceMode2D.Impulse);
            }
            if (transform.position.y <= verticalBoundsBot.y + 5f)
            {
                rig.AddForce(Vector2.up * 2, ForceMode2D.Impulse);
            }

            inputThings();

            float angle = 0;

            if (!rolling)
                angle = Mathf.LerpAngle(transform.rotation.eulerAngles.y, rotLerp.y, Time.deltaTime * 5);

            transform.rotation = Quaternion.Euler(new Vector3(0, angle, 0));
        }

        respawnTime = respawnTime + Time.deltaTime < respawnInvincibility ? respawnTime + Time.deltaTime : respawnInvincibility;

        //Invincible flicker logic
        if (isInvincible)
        {   
            if(flickerDown)
            {
                invincibleFlickerCooldown = Mathf.Max(invincibleFlickerCooldown - Time.deltaTime, 0.0f);
                if (invincibleFlickerCooldown == 0)
                {
                    flickerDown = false;
                }
            }
            else
            {
                invincibleFlickerCooldown = Mathf.Min(invincibleFlickerCooldown + Time.deltaTime, invincibleFlickerRate);
                if (invincibleFlickerCooldown == invincibleFlickerRate)
                {
                    flickerDown = true;
                }
            }
            Color _flickerColour = spriteRenderer.color;
            _flickerColour.a = 0.25f + ((invincibleFlickerCooldown / invincibleFlickerRate) / 0.75f);
            spriteRenderer.color = _flickerColour;
            laserRenderer.color = _flickerColour;
        }
        AdwareAds();
    }

    bool rolling;
    IEnumerator doABarrrelRoll(int dir)
    {
        rolling = true;
        Vector3 initialRot = transform.rotation.eulerAngles;
        float t=0;

        Vector3 currentRot = initialRot;

        while (t < .5f)
        {
            currentRot.y = Mathf.Lerp(initialRot.y, rotLerp.y + ((dir > 0) ? 360 : -90), t / .5f);
            transform.rotation = Quaternion.Euler(currentRot);
            t += Time.deltaTime;
            yield return null;
        }

        rolling = false;
    }

    private void inputThings()
    {
        #region move
        if (Input.GetAxis("Horizontal") < -0.1f)
        {
            if (!rolling)
            {
                if (transform.rotation.eulerAngles.y > 314 && transform.rotation.eulerAngles.y < 317)
                    StartCoroutine(doABarrrelRoll(-1));

                rotLerp = new Vector3(0, 45, 0);
            }
            rig.AddForce(-Vector2.right * speed, ForceMode2D.Impulse);
        }
        else if (Input.GetAxis("Horizontal") > 0.1f)
        {
            if (!rolling)
            {
                if (transform.rotation.eulerAngles.y > 42 && transform.rotation.eulerAngles.y < 45)
                    StartCoroutine(doABarrrelRoll(1));
                rotLerp = new Vector3(0, -45, 0);
            }
            rig.AddForce(Vector2.right * speed, ForceMode2D.Impulse);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (!rolling)
            {
                if (transform.rotation.eulerAngles.y > 314 && transform.rotation.eulerAngles.y < 317)
                    StartCoroutine(doABarrrelRoll(-1));

                rotLerp = new Vector3(0, 45, 0);
            }
            rig.AddForce(-Vector2.right * speed, ForceMode2D.Impulse);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (!rolling)
            {
                if (transform.rotation.eulerAngles.y > 42 && transform.rotation.eulerAngles.y < 45)
                    StartCoroutine(doABarrrelRoll(1));
                rotLerp = new Vector3(0, -45, 0);
            }
            rig.AddForce(Vector2.right * speed, ForceMode2D.Impulse);
        }
        else
        {
            rotLerp = Vector3.zero;
        }

        if (Input.GetAxis("Vertical") > 0.1f)
        {
            rig.AddForce(Vector2.up * speed, ForceMode2D.Impulse);
        }
        else if (Input.GetAxis("Vertical") < -0.1f)
        {
            rig.AddForce(-Vector2.up * speed, ForceMode2D.Impulse);
        }

        #endregion

        #region shoot
        if (Mathf.Abs(Input.GetAxis("Fire1")) > 0.1f)
        {
            if (Shoot(projData, transform.up, shootTransform))
            {
                foreach (ParticleSystem ps in muzzleflash)
                {
                    ps.Emit(1);
                }

                if(backupAvailable)
                {
                    for(int i = 0; i < backups.Length; ++i)
                    {
                        if(backups[i].isActiveAndEnabled)
                        {
                            backups[i].Shoot();
                        }
                    }
                }
                if (projData.explodingBullets)
                    soundManager.instance.playSound(shootSounds[Random.Range(0, shootSounds.Length)], 0.6f);
                else
                {
                    soundManager.instance.playSound(shootSounds[Random.Range(0, shootSounds.Length)],pitchModifier);
                }

                if (CameraShake.instance.shakeDuration < 0.2f)
                {
                    CameraShake.instance.shakeDuration = 0.2f;
                    CameraShake.instance.shakeAmount = 0.15f;
                }
            }
        }
        #endregion
    }
    [Range(0,2)]
    public float pitchModifier = 1;
    public override void TakeDamage(float _damage)
    {
        if(GameStateManager.instance.state == GameStateManager.GameState.GameOver)
        {
            _damage = 0;
        }

        soundManager.instance.playSound(0);
        if (!isInvincible && respawnInvincibility == respawnTime)
        {
            health -= _damage;
            if (health <= 0)
            {
                if (backupAvailable)
                {
                    for(int i = 0; i < backups.Length; ++i)
                    {
                        if(backups[i].isActiveAndEnabled)
                        {
                            transform.position = backups[i].transform.position;
                            backups[i].gameObject.SetActive(false);
                            backups[i].Reset();
                            health = updatedDefaultHealth / 2.0f;
                            PlayerHUD.instance.UpdateHealth(health / updatedDefaultHealth);
                        }
                    }
                }
                else
                {
                    health = updatedDefaultHealth;
                    --lives;
                    PlayerHUD.instance.UpdateLives(lives);
                    //Out of lives then kill player
                    if (lives == 0)
                    {
                        Death();
                    }
                    else
                    {
                        Explosion ex = ExplosionManager.instance.PoolingExplosion(transform, 1);
                        ex.transform.position = transform.position;
                        gameObject.SetActive(false);
                        ex.gameObject.SetActive(true);
                        ex.explode();

                        Invoke("Respawn", 2);
                    }
                }
            }
            PlayerHUD.instance.UpdateHealth(health / updatedDefaultHealth);

            if (CameraShake.instance.shakeDuration < 0.2f)
                CameraShake.instance.shakeDuration += 0.2f;
            CameraShake.instance.shakeAmount = 0.5f;

            if (IsInvoking("revertColour"))
            {
                CancelInvoke("revertColour");
            }
            spriteRenderer.color = Color.red;
            Invoke("revertColour", .1f);
        }
    }

    public override float ActorHealthPercent()
    {
        return health / updatedDefaultHealth;
    }

    protected override void Death()
    {
        PlayerHUD.instance.UpdateHealth(health / updatedDefaultHealth);
        Explosion ex = ExplosionManager.instance.PoolingExplosion(transform, 1);
        ex.transform.position = transform.position;
        gameObject.SetActive(false);
        ex.gameObject.SetActive(true);
        ex.explode();
        soundManager.instance.music.Stop();
        GameStateManager.instance.GameOver();
    }

    void Respawn()
    {
        respawnTime = 0.0f;
        transform.position = verticalBoundsBot + Vector2.up;
        gameObject.SetActive(true);
    }

    public virtual bool Heal(float _heal)
    {
        health = Mathf.Min(health + _heal, updatedDefaultHealth);
        PlayerHUD.instance.UpdateHealth(health / updatedDefaultHealth);
        if(health == updatedDefaultHealth)
        {
            return true;
        }
        return false;
    }

	public override void Reset()
	{
		base.Reset ();
        DestroyBackups();
		updatedDefaultHealth = defaultHealth;
        PlayerHUD.instance.UpdateHealth(health / updatedDefaultHealth);
        lives = defaultLives;
        PlayerHUD.instance.UpdateLives(lives);
        //score = 0;
        //PlayerHUD.instance.UpdateScore(score);
	}

    public int GetScore()
    {
        return score;
    }

    public void IncreaseScore(int _score)
    {
        score += _score;
        PlayerHUD.instance.UpdateScore(score);
    }

	public void HomingProjectiles(bool _homing, float _radius)
	{
		projData.homingBullets = _homing;
		projData.homingRadius = _radius;
        /*foreach (PlayerBackup backup in backups)
        {
            backup.HomingProjectiles(_homing, _radius);
        }*/
    }

	public void ExplodingProjectiles(bool _rocket, float _damage, float _radius)
	{
		projData.explodingBullets = _rocket;
		projData.explosionDamage = _damage;
		projData.explosionRadius = _radius;
	}

    public void ImproveStat(UpdateTypes _type, float _improve)
    {
        switch (_type)
        {
            case UpdateTypes.HEALTH:
                float _percent = health / updatedDefaultHealth;
                updatedDefaultHealth += _improve;
                health = updatedDefaultHealth * _percent;
                break;
            case UpdateTypes.FIRERATE:
                shootRate -= _improve;
                break;
            case UpdateTypes.SPEED:
                speed += _improve;
                break;
            case UpdateTypes.DAMAGE:
                projData.projDamage += _improve;
                break;

        }
    }

    public void SetInvincible(bool _invincible)
    {
        isInvincible = _invincible;
        invincibleFlickerCooldown = invincibleFlickerRate;
        spriteRenderer.color = Color.white;
        flickerDown = true;
    }

    public void SpawnBackups(int _numBackups)
    {
        backupAvailable = true;
        for(int i = 0; i < _numBackups; ++i)
        {
            backups[i].Spawn();
        }
    }

    public bool TestBackups()
    {
        for (int i = 0; i < backups.Length; ++i)
        {
            if(backups[i].isActiveAndEnabled)
            {
                return true;
            }
        }
        backupAvailable = false;
        return false;
    }

    public void DestroyBackups()
    {
        if(backupAvailable)
        {
            for(int i = 0; i < backups.Length; ++i)
            {
                backups[i].gameObject.SetActive(false);
                backups[i].Reset();
            }
            backupAvailable = false;
        }
    }

    void AdwareAds()
    {
        if (advertAttack)
        {
            if (adCool < maxAdCool)
            {
                adCool += Time.deltaTime;
            }
            else
            {
                if (addAmount < randomAdAmount)
                {
                    AdManager.instance.TryGenerateAd(new Vector3(25, 25, 0));
                    addAmount++;
                    adCool = 0;
                }
                else
                {
                    addAmount = 0;
                    adCool = 0;
                    advertAttack = false;
                }
            }
        }
    }

    public void ForceChildObjectsOff()
    {
        foreach(GameObject _obj in childObjects)
        {
            _obj.SetActive(false);
        }
    }
}
