using UnityEngine;
using System.Collections;
using Photon.Pun;

namespace Com.MyCompany.multiTest
{
    public class PlayerAnimatorManager : MonoBehaviourPun
    {
        #region Private Fields

        [SerializeField]
        private float directionDampTime = .25f;
        private Animator animator;

        private float horizontalSpeed = 2.0F;
        private float verticalSpeed = 2.0F;

        #endregion

        #region MonoBehaviour CallBacks

        // Use this for initialization
        void Start()
        {

            animator = GetComponent<Animator>();
            if (!animator)
            {
                Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
            }
            //animator.gameObject.AddComponent<PlayerMovement>().animator = animator;

        }

        // Update is called once per frame
        void Update()
        {
            try
            {
                if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
                {
                    return;
                }

                if (!animator)
                {
                    return;
                }

                //rotacio de la camara en X. 
                //TODO: El moviment Y per ara no es necesari i s'ha d'arreglar
                float X_Mouse_Movement = horizontalSpeed * Input.GetAxis("Mouse X");
                float Y_Mouse_Movement = verticalSpeed * Input.GetAxis("Mouse Y");
                //camara sense mirar adalt i abaix:
                transform.Rotate(0, X_Mouse_Movement, 0);
                //camara amb moviment adalt i abaix:
                //transform.Rotate(-Y_Mouse_Movement/2, X_Mouse_Movement, 0);
                
                // deal with Jumping
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                
                // When using trigger parameter
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    animator.SetBool("Jump", true);
                }
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    animator.SetBool("Jump", false);
                }
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
              
                animator.SetFloat("Speed", v);
                animator.SetFloat("Direction", h * 3, directionDampTime, Time.deltaTime);
               
            }
            catch
            {

            }
        }

        void LateUpdate(){

        }
        #endregion
    }
}
