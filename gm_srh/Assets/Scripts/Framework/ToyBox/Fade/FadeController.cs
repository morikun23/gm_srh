using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ToyBox {
    [AddComponentMenu("ToyBox/Effect/FadeController")]
    public class FadeController : MonoBehaviour {

        [SerializeField]
        private Canvas rootCanvas;

        [SerializeField]
        private Image image;

        [SerializeField]
        private Material material;

        /// <summary>Shaderの色キー</summary>
        private const string COLOR_KEY = "_Color";

        /// <summary>Shaderの拡大キー</summary>
        private const string SCALE_KEY = "_Scale";

        /// <summary>デフォルトのフェード時間</summary>
        private const float DEFAULT_FADE_DURATION = 0.5f;

        /// <summary>デフォルトのフェード色</summary>
        private static readonly Color DEFAULT_FADE_COLOR = Color.white;

        #region Singleton
        private static FadeController instance;

        public static FadeController Instance {
            get {
                if(instance == null) {
                    instance = FindObjectOfType<FadeController>();
                    instance.Initialize();
                }
                return instance;
            }
        }

        private void Awake() {
            if(Instance != this) {
                Destroy(this.gameObject);
            }
        }
        #endregion

        /// <summary>
        /// 初期化処理
        /// </summary>
        private void Initialize() {
            DontDestroyOnLoad(rootCanvas.gameObject);
            image.material = Instantiate(material);
        }

        /// <summary>
        /// フェードイン
        /// デフォルト設定で行う
        /// </summary>
        /// <param name="arg_onComplete">フェード完了時コールバック</param>
        public void FadeIn(System.Action arg_onComplete = null) {
            this.FadeIn(DEFAULT_FADE_DURATION, DEFAULT_FADE_COLOR, arg_onComplete);
        }

        /// <summary>
        /// フェードイン
        /// </summary>
        /// <param name="arg_fadeDuration">フェード速度</param>
        /// <param name="arg_fadeColor">フェード色</param>
        /// <param name="arg_onComplete">フェード完了時コールバック</param>
        public void FadeIn(float arg_fadeDuration,
                           Color arg_fadeColor,
                           System.Action arg_onComplete) {
            image.material.DOColor(arg_fadeColor, COLOR_KEY, 0);
            image.material.DOFloat(1, SCALE_KEY, arg_fadeDuration).OnComplete(() =>{
                if(arg_onComplete != null) arg_onComplete();
                image.gameObject.SetActive(false);
            });
        }

        /// <summary>
        /// フェードアウト
        /// デフォルト設定で行う
        /// </summary>
        /// <param name="arg_onComplete">フェード完了時コールバック</param>
        public void FadeOut(System.Action arg_onComplete = null) {
            this.FadeOut(DEFAULT_FADE_DURATION,DEFAULT_FADE_COLOR, arg_onComplete);
        }

        /// <summary>
        /// フェードアウト
        /// </summary>
        /// <param name="arg_fadeDuration">フェード速度</param>
        /// <param name="arg_fadeColor">フェード色</param>
        /// <param name="arg_onComplete">フェード完了時コールバック</param>
        public void FadeOut(float arg_fadeDuration,
                           Color arg_fadeColor,
                           System.Action arg_onComplete) {
            image.gameObject.SetActive(true);
            image.material.DOColor(arg_fadeColor, COLOR_KEY, 0);
            image.material.DOFloat(0, SCALE_KEY, arg_fadeDuration).OnComplete(() => {
                if(arg_onComplete != null) arg_onComplete();
            });
        }

        /// <summary>
        /// カメラを設定する
        /// </summary>
        /// <param name="arg_camera">Argument camera.</param>
        public virtual void SetCamera(Camera arg_camera){
            rootCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            rootCanvas.worldCamera = arg_camera;
        }

#if UNITY_EDITOR
        private void Reset() {
            image = GetComponentInChildren<Image>();
            rootCanvas = transform.root.GetComponentInChildren<Canvas>();
        }
#endif
    }
}