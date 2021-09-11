using UnityEngine;
using UnityEngine.EventSystems;

using Photon.Pun;

using System.Collections;

namespace Com.Tencent.DYYS
{
    public class PlayerManager : MonoBehaviourPunCallbacks
    {
        #region Private Fields

        #endregion

        #region MonoBehaviour CallBacks

        void Awake()
        {

        }

        void Update()
        {
            ProcessInputs();
        }

        #endregion

        #region Custom

        void ProcessInputs()
        {
            
        }

        #endregion
    }
}