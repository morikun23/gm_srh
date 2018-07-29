using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToyBox {
	[CreateAssetMenu(menuName = "ToyBox/Create ConfigTable", fileName = "ToyBoxConfig")]
	public class Config : ScriptableObject {

		private static readonly string RESOURCE_PATH = "ToyBoxConfig";

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

		private void ExecPreFormat() {
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

		[SerializeField]
		private string _modalPath = "Resources/Modal";

		[SerializeField]
		private string _sePath = "Resources/Audio/Se";

		[SerializeField]
		private string _bgmPath = "Resources/Audio/Bgm";

		public string ModalPath { get { return this._modalPath; } }

		public string SePath { get { return this._sePath; } }

		public string BgmPath { get { return this._bgmPath; } }
	}
}