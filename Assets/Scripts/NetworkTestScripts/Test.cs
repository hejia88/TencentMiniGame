using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class Test : MonoBehaviourPun
{
    void Kill(GameObject PlayerToKill)
    {
        //Do something like play dead animation (auto sync with PhotonAnimatorView Component)
        PhotonView m_PhotonView = PhotonView.Get(this);
        m_PhotonView.RPC("FinishKill", RpcTarget.AllViaServer, PlayerToKill);
    }

    [PunRPC]
    void FinishKill(GameObject playerToKill)
    {
        //Do something like destory player instance
        //Do something like play global ui (xxx was killed)
    }
}
