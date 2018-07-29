using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToyBox {
	[CreateAssetMenu(menuName = "ToyBox/Create ConfigTable", fileName = "ToyBoxConfig")]
	public class Config : ScriptableObject {

		private const string RESOURCE_PATH = "ToyBoxConfig";

		#region Singleton
		private static Config _instance = null;
		
		public static Config Instance {
			get {
				if (_instance == null) {
					Config asset = Resources.Load(RESOURCE_PATH) as Config;
					if (asset == null) {
						Debug.AssertFormat(false, "Missing ParameterTable! path={0}", RESOURCE_PATH);
						asset = CreateInstance<Config>();
					}
					_instance = asset;
					_instance.ExecPreFormat();
				}
				return _instance;
			}
		}
		
		private Config() { }
		#endregion

		[System.Serializable]
		private class ResourcesPath {
		
			[SerializeField]
			public string _modalPath = "Resources/Modal";
	
			[SerializeField]
			public string _sePath = "Resources/Audio/Se";
	
			[SerializeField]
			public string _bgmPath = "Resources/Audio/Bgm";

			/// <summary>
			/// フォーマットを整える
			/// </summary>
			public void ExecFormat() {
				if (this._modalPath.EndsWith("/", System.StringComparison.Ordinal)) {
					_modalPath = _modalPath.Remove(_modalPath.Length - 1);
				}
				if (this._sePath.EndsWith("/", System.StringComparison.Ordinal)) {
					_sePath = _sePath.Remove(_sePath.Length - 1);
				}
				if (this._bgmPath.EndsWith("/", System.StringComparison.Ordinal)) {
					_bgmPath = _bgmPath.Remove(_bgmPath.Length - 1);
				}
			}
		}
		
		[SerializeField]
		private bool isEnableDebugMode = true;

		[SerializeField]
		private ResourcesPath resourcesPath;
		
		public bool IsEnableDebugMode { get { return this.isEnableDebugMode; } }
		
		public string ModalPath { get { return this.resourcesPath._modalPath; } }

		public string SePath { get { return this.resourcesPath._sePath; } }

		public string BgmPath { get { return this.resourcesPath._bgmPath; } }
		
		
		/// <summary>
		/// 実行前にフォーマットを整える
		/// </summary>
		private void ExecPreFormat() {
			this.resourcesPath.ExecFormat();
		}
	}
}