using Unity.FPS.Game;
using UnityEngine;

namespace Unity.FPS.AI
{
    [RequireComponent(typeof(EnemyController))]
    public class EnemyTurret : MonoBehaviour
    {
        public enum AIState
        {
            Idle,
            Attack,
        }

        [Header("Turret Setup")]
        public Transform TurretPivot;
        public Transform TurretAimPoint;
        public Animator Animator;

        [Header("Rotation")]
        public float AimRotationSharpness = 5f;
        public float LookAtRotationSharpness = 2.5f;

        [Header("Combat")]
        public float DetectionFireDelay = 1f;
        public float AimingTransitionBlendTime = 1f;

        [Header("Effects")]
        public ParticleSystem[] RandomHitSparks;
        public ParticleSystem[] OnDetectVfx;
        public AudioClip OnDetectSfx;

        public AIState AiState { get; private set; }

        EnemyController m_EnemyController;
        Health m_Health;

        Quaternion m_RotationWeaponForwardToPivot;

        float m_TimeStartedDetection;
        float m_TimeLostDetection;

        Quaternion m_PreviousPivotAimingRotation;
        Quaternion m_PivotAimingRotation;

        const string k_AnimOnDamagedParameter = "OnDamaged";
        const string k_AnimIsActiveParameter = "IsActive";

        void Awake()
        {
            m_Health = GetComponent<Health>();
            m_EnemyController = GetComponent<EnemyController>();

            DebugUtility.HandleErrorIfNullGetComponent<Health, EnemyTurret>(m_Health, this, gameObject);
            DebugUtility.HandleErrorIfNullGetComponent<EnemyController, EnemyTurret>(m_EnemyController, this, gameObject);
        }

        void Start()
        {
            m_Health.OnDamaged += OnDamaged;

            m_EnemyController.onDetectedTarget += OnDetectedTarget;
            m_EnemyController.onLostTarget += OnLostTarget;

            m_RotationWeaponForwardToPivot =
                Quaternion.Inverse(m_EnemyController.GetCurrentWeapon().WeaponMuzzle.rotation) *
                TurretPivot.rotation;

            AiState = AIState.Idle;

            m_TimeStartedDetection = Mathf.NegativeInfinity;
            m_TimeLostDetection = Mathf.NegativeInfinity;

            m_PreviousPivotAimingRotation = TurretPivot.rotation;
            m_PivotAimingRotation = TurretPivot.rotation;
        }

        void Update()
        {
            UpdateCurrentAiState();
        }

        void LateUpdate()
        {
            UpdateTurretAiming();
        }

        void UpdateCurrentAiState()
        {
            switch (AiState)
            {
                case AIState.Attack:

                    if (m_EnemyController == null ||
                        m_EnemyController.KnownDetectedTarget == null)
                    {
                        AiState = AIState.Idle;
                        return;
                    }

                    bool mustShoot = Time.time >
                                     m_TimeStartedDetection + DetectionFireDelay;

                    Vector3 directionToTarget =
                        (m_EnemyController.KnownDetectedTarget.transform.position -
                         TurretAimPoint.position);

                    if (directionToTarget.sqrMagnitude < 0.001f)
                        return;

                    directionToTarget.Normalize();

                    Quaternion targetRotation =
                        Quaternion.LookRotation(directionToTarget) *
                        m_RotationWeaponForwardToPivot;

                    float rotationSpeed = mustShoot ?
                        AimRotationSharpness :
                        LookAtRotationSharpness;

                    m_PivotAimingRotation =
                        Quaternion.Slerp(
                            m_PreviousPivotAimingRotation,
                            targetRotation,
                            rotationSpeed * Time.deltaTime);

                    if (mustShoot)
                    {
                        Vector3 correctedDirectionToTarget =
                            (m_PivotAimingRotation *
                             Quaternion.Inverse(m_RotationWeaponForwardToPivot)) *
                            Vector3.forward;

                        m_EnemyController.TryAtack(
                            TurretAimPoint.position + correctedDirectionToTarget);
                    }

                    break;
            }
        }

        void UpdateTurretAiming()
        {
            switch (AiState)
            {
                case AIState.Attack:

                    TurretPivot.rotation = m_PivotAimingRotation;
                    break;

                default:

                    TurretPivot.rotation = Quaternion.Slerp(
                        m_PivotAimingRotation,
                        TurretPivot.rotation,
                        (Time.time - m_TimeLostDetection) /
                        AimingTransitionBlendTime);

                    break;
            }

            m_PreviousPivotAimingRotation = TurretPivot.rotation;
        }

        void OnDamaged(float dmg, GameObject source)
        {
            if (RandomHitSparks.Length > 0)
            {
                int n = Random.Range(0, RandomHitSparks.Length);
                RandomHitSparks[n].Play();
            }

            if (Animator != null)
                Animator.SetTrigger(k_AnimOnDamagedParameter);
        }

        void OnDetectedTarget()
        {
            Debug.Log("Turret detected player: " + name);

            if (AiState == AIState.Idle)
                AiState = AIState.Attack;

            foreach (var vfx in OnDetectVfx)
            {
                if (vfx != null)
                    vfx.Play();
            }

            if (OnDetectSfx)
            {
                AudioUtility.CreateSFX(
                    OnDetectSfx,
                    transform.position,
                    AudioUtility.AudioGroups.EnemyDetection,
                    1f);
            }

            if (Animator != null)
                Animator.SetBool(k_AnimIsActiveParameter, true);

            m_TimeStartedDetection = Time.time;
        }

        void OnLostTarget()
        {
            if (AiState == AIState.Attack)
                AiState = AIState.Idle;

            foreach (var vfx in OnDetectVfx)
            {
                if (vfx != null)
                    vfx.Stop();
            }

            if (Animator != null)
                Animator.SetBool(k_AnimIsActiveParameter, false);

            m_TimeLostDetection = Time.time;
        }

        public void ResetTurret()
        {
            Debug.Log("Reset turret: " + name);

            AiState = AIState.Idle;

            m_TimeStartedDetection = Mathf.NegativeInfinity;
            m_TimeLostDetection = Time.time;

            if (Animator != null)
            {
                Animator.Rebind();
                Animator.Update(0f);
                Animator.SetBool(k_AnimIsActiveParameter, false);
            }

            if (OnDetectVfx != null)
            {
                foreach (var vfx in OnDetectVfx)
                {
                    if (vfx != null)
                        vfx.Stop();
                }
            }

            Collider[] colliders =
                GetComponentsInChildren<Collider>();

            foreach (var col in colliders)
            {
                if (col != null)
                    col.enabled = true;
            }

            if (m_Health != null)
                m_Health.CurrentHealth = m_Health.MaxHealth;

            gameObject.SetActive(true);
        }
    }
}