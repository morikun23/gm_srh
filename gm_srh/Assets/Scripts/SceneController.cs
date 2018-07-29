using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using ToyBox;

public enum GameScene {
    StartUp,
    Title,
    Home,
    SearchGame,
    Collection,
    Memory
}

public class SceneController : MonoBehaviour {

    /// <summary>シーンファイルの接頭辞</summary>
    protected const string SCENE = "Scene_";

    /// <summary>背景用レイヤー</summary>
    private const string BACK_GROUND_LAYER = "BackGround";

    /// <summary>通常レイヤー</summary>
    private const string DEFAULT_LAYER = "Default";

    /// <summary>イベントなど強調用レイヤー</summary>
    private const string FRONT_LAYER = "Front";

    /// <summary>モーダル用レイヤー</summary>
    private const string MODAL_LAYER = "Modal";

    /// <summary>システム用レイヤー</summary>
    private const string SYSTEM_LAYER = "System";

    [System.Serializable]
    private class LayerCamera {

        /// <summary>BackGround Layer</summary>
        public Camera bgCamera;

        /// <summary>Default Layer</summary>
        public Camera defaultCamera;

        /// <summary>Front Layer</summary>
        public Camera frontCamera;

        /// <summary>Modal Layer</summary>
        public Camera modalCamera;

        /// <summary>System Layer</summary>
        public Camera systemCamera;
    }

    [SerializeField]
    private LayerCamera layerCameras;

    /// <summary>BackGround Layer</summary>
    public Camera BgCamera {
        get { return layerCameras.bgCamera; }
    }

    /// <summary>Default Layer</summary>
    public Camera DefaultCamera {
        get { return layerCameras.defaultCamera; }
    }

    /// <summary>Front Layer</summary>
    public Camera FrontCamera {
        get { return layerCameras.frontCamera; }
    }

    /// <summary>Modal Layer</summary>
    public Camera ModalCamera {
        get { return layerCameras.modalCamera; }
    }

    /// <summary>System Layer</summary>
    public Camera SystemCamera {
        get { return layerCameras.systemCamera; }
    }

    protected void Start() {
        this.OnSceneLoad();
        FadeController.Instance.SetCamera(layerCameras.systemCamera);
        FadeController.Instance.FadeIn(
            () =>{
                this.OnSceneEnter();
            }
        );
    }

    protected void Update() {
        this.OnSceneUpdate();
    }

    /// <summary>
    /// シーン遷移を行う
    /// </summary>
    /// <param name="arg_gameScene">遷移するシーン名</param>
    public void SceneTransition(GameScene arg_gameScene) {
        this.OnSceneExit();
        FadeController.Instance.FadeOut(() => {
            this.OnSceneUnLoad();
            SceneManager.LoadScene(SCENE + arg_gameScene);
        });
    }

    /// <summary>
    /// シーン遷移後フェード解除前
    /// </summary>
    protected virtual void OnSceneLoad() { }

    /// <summary>
    /// シーン遷移後フェード解除完了時
    /// </summary>
    protected virtual void OnSceneEnter() { }

    /// <summary>
    /// シーン有効時毎フレーム実行
    /// </summary>
    protected virtual void OnSceneUpdate() { }

    /// <summary>
    /// シーン遷移前フェード開始前
    /// </summary>
    protected virtual void OnSceneExit() { }

    /// <summary>
    /// シーン遷移直前フェード完了時
    /// </summary>
    protected virtual void OnSceneUnLoad() { }

#if UNITY_EDITOR
    protected void Reset() {
        
    }

    [ContextMenu("カメラの登録")]
    public void SetLayerCameras(){
        Dictionary<string, Camera> cameraDict = new Dictionary<string, Camera>();
        foreach(Camera gameCamera in FindObjectsOfType<Camera>()) {
            cameraDict.Add(LayerMask.LayerToName(gameCamera.gameObject.layer), gameCamera);
        }

        layerCameras.bgCamera = cameraDict[BACK_GROUND_LAYER];
        layerCameras.defaultCamera = cameraDict[DEFAULT_LAYER];
        layerCameras.frontCamera = cameraDict[FRONT_LAYER];
        layerCameras.modalCamera = cameraDict[MODAL_LAYER];
        layerCameras.systemCamera = cameraDict[SYSTEM_LAYER];
    }
#endif
}
