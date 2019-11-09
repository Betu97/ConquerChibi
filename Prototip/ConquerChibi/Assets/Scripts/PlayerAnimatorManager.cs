﻿using UnityEngine;
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
                // deal with Jumping
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                // only allow jumping if we are running.
                if (stateInfo.IsName("Base Layer.Run"))
                {
                    // When using trigger parameter
                    if (Input.GetButtonDown("Fire2"))
                    {
                        animator.SetTrigger("Jump");
                    }
                }
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
                if (v < 0)
                {
                    v = 0;
                }
                animator.SetFloat("Speed", h * h + v * v);
                animator.SetFloat("Direction", h, directionDampTime, Time.deltaTime);
            }
            catch
            {

            }
            
        }



        #endregion
    }
}