using System.Security.Policy;
using Assets.Scripts.Enemy;
using Assets.Scripts.Manager;
//using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class PlayerShooting : MonoBehaviour
    {

        public int DamagePerShoot = 20;
        public float TimeBetweenBullets = 0.15f;
        public float Range=100;

        private float _timer;
        private Ray _shootRay;
        private RaycastHit _shootHit;
        private int _shootableMask;
        private ParticleSystem _gunParticles;
        private AudioSource _gunAudio;
        private LineRenderer _gunLine;
        private Light _gunLight;   
        private const float EffectDisplayTime=0.2f;
        public VirtualJoystick JoysticShoot;

        //Control vars

        public int shootCount = 0;
        public int shootHitCount = 0;
        public float shootAverage = 0;
        public float gameTimer = 0;    //Gameplay Timer


        private void Awake()
        {
            _shootableMask = LayerMask.GetMask("Shootable");
            _gunParticles = GetComponent<ParticleSystem>();
            _gunLine = GetComponent<LineRenderer>();
            _gunAudio = GetComponent<AudioSource>();
            _gunLight = GetComponent<Light>();

        }

        private void Update()
        {
            _timer += Time.deltaTime;
            gameTimer += Time.deltaTime;
            if ((/*Input.GetButton("Fire1") || */JoysticShoot.IsShooting()) && _timer >= TimeBetweenBullets)
                Shoot();
            if (_timer >= TimeBetweenBullets*EffectDisplayTime)
                DisableEffects();
        }

        public void DisableEffects()
        {
            _gunLine.enabled = false;
            _gunLight.enabled = false;
        }

        public float HitRate()
        {
            shootAverage = shootHitCount / shootCount * 100;
            return shootAverage;
        }

        private void Shoot()
        {
            _timer = 0;
            _gunAudio.Play();
            _gunLight.enabled = true;
            _gunParticles.Stop();
            _gunParticles.Play();


            //SHOOT COUNTER
            shootCount++;

            _gunLine.enabled = true;
            _gunLine.SetPosition(0,transform.position);
            
            _shootRay.origin = transform.position;
            _shootRay.direction =transform.forward;

            if (Physics.Raycast(_shootRay, out _shootHit, Range, _shootableMask))
            {
                EnemyHealth enemyHealth = _shootHit.collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                    enemyHealth.TakeDamage(DamagePerShoot, _shootHit.point);
                _gunLine.SetPosition(1, _shootHit.point);

                //HIT COUNTER
                shootHitCount++;
            }
            else
            {
                _gunLine.SetPosition(1,_shootRay.origin +_shootRay.direction*Range);
                
            }
        }
    }
}
