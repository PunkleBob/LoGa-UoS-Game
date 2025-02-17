using UnityEngine.Scripting.APIUpdating;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

namespace UnityEngine.XR.Interaction.Toolkit.Locomotion.Climbing
{
    /// <summary>
    /// Interactable that can be climbed while selected.
    /// </summary>
    /// <seealso cref="ClimbProvider"/>
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Rigidbody))]
    [AddComponentMenu("XR/Climb Interactable", 11)]
    [HelpURL(XRHelpURLConstants.k_ClimbInteractable)]
    [MovedFrom("UnityEngine.XR.Interaction.Toolkit")]
    public class ClimbInteractable : XRBaseInteractable
    {
        const float k_DefaultMaxInteractionDistance = 0.1f;

        [SerializeField]
        [Tooltip("The climb provider that performs locomotion while this interactable is selected. " +
                 "If no climb provider is configured, will attempt to find one.")]
        ClimbProvider m_ClimbProvider;

        /// <summary>
        /// The climb provider that performs locomotion while this interactable is selected.
        /// If no climb provider is configured, will attempt to find one.
        /// </summary>
        public ClimbProvider climbProvider
        {
            get => m_ClimbProvider;
            set => m_ClimbProvider = value;
        }

        [SerializeField]
        [Tooltip("Transform that defines the coordinate space for climb locomotion. " +
                 "Will use this GameObject's Transform by default.")]
        Transform m_ClimbTransform;

        /// <summary>
        /// Transform that defines the coordinate space for climb locomotion. Will use this GameObject's Transform by default.
        /// </summary>
        public Transform climbTransform
        {
            get
            {
                if (m_ClimbTransform == null)
                    m_ClimbTransform = transform;
                return m_ClimbTransform;
            }
            set => m_ClimbTransform = value;
        }

        [SerializeField]
        [Tooltip("Controls whether to apply a distance check when validating hover and select interaction.")]
        bool m_FilterInteractionByDistance = true;

        /// <summary>
        /// Controls whether to apply a distance check when validating hover and select interaction.
        /// </summary>
        /// <seealso cref="maxInteractionDistance"/>
        /// <seealso cref="XRBaseInteractable.distanceCalculationMode"/>
        public bool filterInteractionByDistance
        {
            get => m_FilterInteractionByDistance;
            set => m_FilterInteractionByDistance = value;
        }

        [SerializeField]
        [Tooltip("The maximum distance that an interactor can be from this interactable to begin hover or select.")]
        float m_MaxInteractionDistance = k_DefaultMaxInteractionDistance;

        /// <summary>
        /// The maximum distance that an interactor can be from this interactable to begin hover or select.
        /// Only applies when <see cref="filterInteractionByDistance"/> is <see langword="true"/>.
        /// </summary>
        /// <seealso cref="filterInteractionByDistance"/>
        /// <seealso cref="XRBaseInteractable.distanceCalculationMode"/>
        public float maxInteractionDistance
        {
            get => m_MaxInteractionDistance;
            set => m_MaxInteractionDistance = value;
        }

        [SerializeField]
        [Tooltip("The teleport volume used to assist with movement to a specific destination after ending a climb " +
            "(optional, may be None). Only used if there is a Climb Teleport Interactor in the scene.")]
        TeleportationMultiAnchorVolume m_ClimbAssistanceTeleportVolume;

        /// <summary>
        /// The teleport volume used to assist with movement to a specific destination after ending a climb (optional,
        /// may be <see langword="null"/>). If there is a <see cref="ClimbTeleportInteractor"/> in the scene that
        /// references the same <see cref="ClimbProvider"/> as this interactable, it will interact with the volume while
        /// this interactable is being climbed.
        /// </summary>
        public TeleportationMultiAnchorVolume climbAssistanceTeleportVolume
        {
            get => m_ClimbAssistanceTeleportVolume;
            set => m_ClimbAssistanceTeleportVolume = value;
        }

        [SerializeField]
        [Tooltip("Optional override of locomotion settings specified in the climb provider. " +
                 "Only applies as an override if set to Use Value or if the asset reference is set.")]
        ClimbSettingsDatumProperty m_ClimbSettingsOverride;

        /// <summary>
        /// Optional override of climb locomotion settings specified in the climb provider. Only applies as
        /// an override if <see cref="Unity.XR.CoreUtils.Datums.DatumProperty{TValue, TDatum}.Value"/> is not <see langword="null"/>.
        /// </summary>
        public ClimbSettingsDatumProperty climbSettingsOverride
        {
            get => m_ClimbSettingsOverride;
            set => m_ClimbSettingsOverride = value;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnValidate()
        {
            if (m_ClimbTransform == null)
                m_ClimbTransform = transform;
        }

        /// <inheritdoc />
        protected override void Reset()
        {
            base.Reset();

            selectMode = InteractableSelectMode.Multiple;
            m_ClimbTransform = transform;
        }

        /// <inheritdoc />
        protected override void Awake()
        {
            base.Awake();
            if (m_ClimbProvider == null)
                ComponentLocatorUtility<ClimbProvider>.TryFindComponent(out m_ClimbProvider, true);
        }

        /// <inheritdoc />
        public override bool IsHoverableBy(IXRHoverInteractor interactor)
        {
            return base.IsHoverableBy(interactor) && (!m_FilterInteractionByDistance ||
                GetDistanceSqrToInteractor(interactor) <= m_MaxInteractionDistance * m_MaxInteractionDistance);
        }

        /// <inheritdoc />
        public override bool IsSelectableBy(IXRSelectInteractor interactor)
        {
            return base.IsSelectableBy(interactor) && (IsSelected(interactor) || !m_FilterInteractionByDistance ||
                GetDistanceSqrToInteractor(interactor) <= m_MaxInteractionDistance * m_MaxInteractionDistance);
        }

        /// <inheritdoc />
        protected override void OnSelectEntered(SelectEnterEventArgs args)
        {
            base.OnSelectEntered(args);
            if (m_ClimbProvider != null || ComponentLocatorUtility<ClimbProvider>.TryFindComponent(out m_ClimbProvider))
                m_ClimbProvider.StartClimbGrab(this, args.interactorObject);
        }

        /// <inheritdoc />
        protected override void OnSelectExited(SelectExitEventArgs args)
        {
            base.OnSelectExited(args);
            if (m_ClimbProvider != null)
                m_ClimbProvider.FinishClimbGrab(args.interactorObject);
        }
    }
}
