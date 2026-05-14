using UnityEngine;

namespace TextboxControl
{
    /// <summary>
    /// Bridges face-event pills (Mouth method 20, Eye method 21) fired during
    /// dialogue playback to the <see cref="CustomerAnimation"/> on the current
    /// customer. Attach to any scene object; wire up <see cref="textboxController"/>.
    /// </summary>
    public class DialogueFaceEventHandler : MonoBehaviour
    {
        [SerializeField] private TextboxController textboxController;

        private void OnEnable()
        {
            if (textboxController != null)
                textboxController.OnExternalControl += HandleExternalControl;
        }

        private void OnDisable()
        {
            if (textboxController != null)
                textboxController.OnExternalControl -= HandleExternalControl;
        }

        private void HandleExternalControl(int method, string[] parameters)
        {
            CustomerAnimation anim = GetCurrentAnimation();
            if (anim == null) return;

            string state = parameters.Length > 0 ? parameters[0] : string.Empty;

            switch ((MethodCode)method)
            {
                case MethodCode.Mouth:
                    anim.ApplyMouthState(state);
                    break;
                case MethodCode.Eye:
                    anim.ApplyEyeState(state);
                    break;
            }
        }

        private static CustomerAnimation GetCurrentAnimation()
        {
            Customer customer = CustomerLineManager.Instance?.CurrentCustomer;
            return customer != null ? customer.GetComponentInChildren<CustomerAnimation>() : null;
        }
    }
}
