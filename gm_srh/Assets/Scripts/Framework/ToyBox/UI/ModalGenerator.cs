using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToyBox {
    /// <summary>
    /// モーダル生成に必要な機能が埋め込まれています
    /// 実際に使用する際はこれの派生クラスに生成処理を記述してください
    /// </summary>
	public class ModalGenerator : MonoBehaviour {

        /// <summary>モーダル出力用カメラ</summary>
        public Camera modalCamera;

        /// <summary>モーダルプレハブのキャッシュ</summary>
        private static Dictionary<string, GameObject> modalDict;

        /// <summary>
        /// モーダルプレハブのキャッシュ
        /// </summary>
        private static Dictionary<string,GameObject> ModalDict {
            get{
                if(modalDict == null){
                    modalDict = GetModalPrefabs();
                }
                return modalDict;
            }
        }

        /// <summary>
        /// モーダルのプレハブを一括取得
        /// </summary>
        private static Dictionary<string,GameObject> GetModalPrefabs() {

            Dictionary<string, GameObject> modals = new Dictionary<string, GameObject>();

            #region Modalフォルダの中身をすべて取得
            GameObject[] modalList = Resources.LoadAll<GameObject>(Config.Instance.ModalPath);
            foreach(GameObject modal in modalList) {
                modals[modal.name] = modal;
            }
            #endregion

            return modals;
        }

        /// <summary>
        /// モーダルを生成する
        /// 生成時にモーダルのデフォルト設定を適用させる
        /// </summary>
        /// <param name="prefabName">生成するモーダルのプレハブファイル名</param>
        /// <param name="layerName">モーダルのレイヤー指定</param>
        protected ModalController InstantiateModal(string prefabName,string layerName = "") {
            return this.InstantiateModal(ModalDict[prefabName],layerName);
        }

        /// <summary>
        /// モーダルを生成する
        /// 生成時にモーダルのデフォルト設定を適用させる
        /// </summary>
        /// <param name="modalPrefab">生成するモーダルのプレハブファイル</param>
        /// <param name="layerName">モーダルのレイヤー指定</param>
        protected ModalController InstantiateModal(GameObject modalPrefab,string layerName ="") {
            
            Canvas canvas = Instantiate(modalPrefab).GetComponent<Canvas>();

            if(canvas == null) {
                Debug.LogError("[ToyBox]プレハブにCanvasを設定してください");
                return null;
            }

            //Show関数のタイミングで表示できるように最初は非表示
            canvas.gameObject.SetActive(false);

            if(modalCamera != null) {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = modalCamera;
                canvas.sortingLayerName = layerName;
            }
            else{
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }

            return canvas.GetComponentInChildren<ModalController>();

        }
	}
}