﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace SD {
    public class SDReadySceneManager : MonoBehaviour {

        /*private SDConnectionManager cManager; TODO
        private SDMessageQueue mQueue;*/
        private static SDReadySceneManager sceneManager;
        private bool _isPlayerReady, _isOpponentReady;
        private bool isPlayerReady { 
            get {
                return this._isPlayerReady;
            }
            set {
                this._isPlayerReady = value;
                if (this._isPlayerReady && this.isOpponentReady) {
                    Debug.Log ("Loading the Game Scene..");
                    SceneManager.LoadScene ("SDGameMain");
                }
            }
        }
        private bool isOpponentReady {
            get {
                return this._isOpponentReady;
            }
            set {
                this._isOpponentReady = value;
                if (this._isPlayerReady && this._isOpponentReady) {
                    Debug.Log ("Loading the Game Scene..");
                    SceneManager.LoadScene ("SDGameMain");
                }
            }
        }

        public static SDReadySceneManager getInstance() {
            return sceneManager;
        }

        // Use this for initialization
        void Start () {
            sceneManager = this;
            /*cManager = SDConnectionManager.getInstance (); tODO
            mQueue = SDMessageQueue.getInstance ();*/
            isPlayerReady = false;
            isOpponentReady = false;

            if (SDMain.networkManager != null) {
                //SDMain.networkManager.Listen (NetworkCode.SD_START_GAME, ResponseSDStartGame);
                //SDMain.networkManager.Listen (NetworkCode.SD_PLAYER_POSITION, ResponseSDStartSync);
            }else {
                Debug.LogWarning ("Could not obtain a connection to Sea Divided Server. Falling back to offline mode.");
            }
            /*if (cManager && mQueue) { TODO
                if (!mQueue.callbackList.ContainsKey(Constants.SMSG_SDSTART_GAME))
                    mQueue.AddCallback (Constants.SMSG_SDSTART_GAME, ResponseSDStartGame);
                if (mQueue.callbackList.ContainsKey(Constants.SMSG_POSITION))  // Remove when we create a new protocol.
                    mQueue.RemoveCallback (Constants.SMSG_POSITION);
                if (mQueue.callbackList.ContainsKey (Constants.SMSG_POSITION))
                    mQueue.RemoveCallback (Constants.SMSG_POSITION);
                if (!mQueue.callbackList.ContainsKey (Constants.SMSG_POSITION))
                    mQueue.AddCallback (Constants.SMSG_POSITION, ResponseSDStartSync);
            } else {
                Debug.LogWarning ("Could not obtain a connection to Sea Divided Server. Falling back to offline mode.");
            }*/
        }

        public void StartGame() {
            if (SDMain.networkManager != null) {
                SDMain.networkManager.Send (SDStartGameProtocol.Prepare (Constants.USER_ID), ResponseSDStartGame);
                /*RequestSDStartGame request = new RequestSDStartGame ();
                request.Send (Constants.USER_ID);
                cManager.Send (request);*/
                isPlayerReady = true;
            } else {
                Debug.LogWarning ("Starting game without server component.");
                SceneManager.LoadScene ("SDGameMain");
            }
        }

        public void ResponseSDStartGame (NetworkResponse r)
        {
            ResponseSDStartGame response = r as ResponseSDStartGame;
            Debug.Log ("ResponseSDStartGame called.");
            /*ResponseSDStartGameEventArgs args = eventArgs as ResponseSDStartGameEventArgs;*/ 

            if (response.status == 0) {
                // Send a request to the opponent indicating that this client is ready to play.
                SDMain.networkManager.Send (SDPlayerPositionProtocol.Prepare (
                    0.ToString (), 0.ToString (), 0.ToString()), ResponseSDStartSync);
                /*RequestSDPosition request = new RequestSDPosition();
                request.Send (0.ToString (), 0.ToString (), 0.ToString());
                cManager.Send (request);*/
                Debug.Log ("Waiting for opponent to respond..");
            } else {
                Debug.Log ("Encountered an error in starting the game.");
            }
        }

        public void ResponseSDStartSync(NetworkResponse r) {
            ResponseSDPlayerPosition response = r as ResponseSDPlayerPosition;
            Debug.Log ("The opponent is ready to play, loading the game scene.");
            this.isOpponentReady = true;
        }
    }
}
