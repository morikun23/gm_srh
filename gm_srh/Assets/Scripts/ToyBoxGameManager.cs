using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToyBox {
	public class ToyBoxGameManager : MonoBehaviour {

		#region Singleton
		private static ToyBoxGameManager _instance;

		public static ToyBoxGameManager Instance {
			get {
				if (_instance == null) {
					_instance = FindObjectOfType<ToyBoxGameManager>();
					if (_instance == null) {
						AppDebug.LogError("[ToyBox]GameManagerが見つかりません");
					}
				}
				return _instance;
			}
		}

		private ToyBoxGameManager() { }
		#endregion


		[System.Serializable]
		private class LayerCamera {

			/// <summary>BackGround Layer</summary>
			[SerializeField]
			public Camera _bgCamera;

			/// <summary>Default Layer</summary>
			[SerializeField]
			public Camera _defaultCamera;

			/// <summary>Front Layer</summary>
			[SerializeField]
			public Camera _frontCamera;

			/// <summary>Modal Layer</summary>
			[SerializeField]
			public Camera _modalCamera;

			/// <summary>System Layer</summary>
			[SerializeField]
			public Camera _systemCamera;
		}

		/// <summary>背景用レイヤー</summary>
		public const string BACK_GROUND_LAYER = "BackGround";

		/// <summary>通常レイヤー</summary>
		public const string DEFAULT_LAYER = "Default";

		/// <summary>イベントなど強調用レイヤー</summary>
		public const string FRONT_LAYER = "Front";

		/// <summary>モーダル用レイヤー</summary>
		public const string MODAL_LAYER = "Modal";

		/// <summary>システム用レイヤー</summary>
		public const string SYSTEM_LAYER = "System";

		[SerializeField]
		private LayerCamera _layerCameras;

		/// <summary>BackGround Layer</summary>
		public Camera BgCamera {
			get { return _layerCameras._bgCamera; }
		}

		/// <summary>Default Layer</summary>
		public Camera DefaultCamera {
			get { return _layerCameras._defaultCamera; }
		}

		/// <summary>Front Layer</summary>
		public Camera FrontCamera {
			get { return _layerCameras._frontCamera; }
		}

		/// <summary>Modal Layer</summary>
		public Camera ModalCamera {
			get { return _layerCameras._modalCamera; }
		}

		/// <summary>System Layer</summary>
		public Camera SystemCamera {
			get { return _layerCameras._systemCamera; }
		}

		/// <summary>
		/// 指定されたレイヤーに対応したカメラを取得する
		/// </summary>
		/// <returns>存在しない場合はDefaultCameraが返る</returns>
		/// <param name="layerName">レイヤー名</param>
		public Camera GetLayerCamera(string layerName) {
			switch (layerName) {
				case BACK_GROUND_LAYER: return BgCamera;
				case DEFAULT_LAYER: 	return DefaultCamera;
				case FRONT_LAYER:		return FrontCamera;
				case MODAL_LAYER: 		return ModalCamera;
				case SYSTEM_LAYER: 		return SystemCamera;
				default: 				return DefaultCamera;
			}
		}
		
		#if UNITY_EDITOR
		    protected void Reset() {
				this.SetLayerCameras();
		    }
		
		    [ContextMenu("カメラの登録")]
		    public void SetLayerCameras(){
		        Dictionary<string, Camera> cameraDict = new Dictionary<string, Camera>();
		        foreach(Camera gameCamera in FindObjectsOfType<Camera>()) {
		            cameraDict.Add(LayerMask.LayerToName(gameCamera.gameObject.layer), gameCamera);
		        }
		
		        _layerCameras._bgCamera 		= cameraDict[BACK_GROUND_LAYER];
		        _layerCameras._defaultCamera 	= cameraDict[DEFAULT_LAYER];
		        _layerCameras._frontCamera 		= cameraDict[FRONT_LAYER];
		        _layerCameras._modalCamera 		= cameraDict[MODAL_LAYER];
		        _layerCameras._systemCamera 	= cameraDict[SYSTEM_LAYER];
		    }
		#endif
	}
}