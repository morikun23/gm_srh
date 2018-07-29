using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ToyBox {
	[AddComponentMenu("ToyBox/Audio/AudioManager")]
	public class AudioManager:MonoBehaviour {

		/// <summary>
		/// 管理しているAudio全体の設定
		/// </summary>
		[System.Serializable]
		private class AudioOption {
			[Range(0.0f , 1.0f),Tooltip("SEの音量")]
			public float seVolume = 1.0f;

			[Range(0.0f , 1.0f),Tooltip("BGMの音量")]
			public float bgmVolume = 0.7f;

			[HideInInspector]
            public Coroutine fadeCoroutine;
		}

		#region Singleton
        private static AudioManager instance;

		public static AudioManager Instance {
			get {
				if(instance == null) {
					instance = FindObjectOfType<AudioManager>();
				}
				return instance;
			}
		}

		private AudioManager() { }
		#endregion

		/// <summary>Resourcesフォルダに格納されているSEファイル</summary>
        private readonly Dictionary<string,AudioClip> seBuffer = new Dictionary<string , AudioClip>();
		
		/// <summary>Resourcesフォルダに格納されているBGMファイル</summary>
        private readonly Dictionary<string , AudioClip> bgmBuffer = new Dictionary<string , AudioClip>();

		/// <summary>現在再生されているAudioSource</summary>
        private readonly Dictionary<string , AudioSource> activeAudioSources = new Dictionary<string , AudioSource>();

		/// <summary>オブジェクトプール</summary>
        private readonly Queue<AudioSource> seSourcePool = new Queue<AudioSource>();

		/// <summary>BGM用のAudioSource</summary>
        private AudioSource bgmSource;

		//再生できるSEの量
		[SerializeField,Tooltip("再生できるSEの量")]
        private uint seSize = 20;

		//再生するAudioSourceの設定
		[SerializeField]
        private AudioOption audioOption;
		
		/// <summary>
		/// UnityEvent
		/// 起動時に処理される
		/// </summary>
		private void Awake() {

			if(Instance != this) {
				Destroy(this.gameObject);
				return;
			} 

			#region オーディオファイルをキャッシュする
			//SEフォルダの中身を全て取得
            AudioClip[] seList = Resources.LoadAll<AudioClip>(Config.Instance.SePath);
			foreach(AudioClip clip in seList) {
				seBuffer[clip.name] = clip;
			}

			//BGMフォルダの中身を全て取得
            AudioClip[] bgmList = Resources.LoadAll<AudioClip>(Config.Instance.BgmPath);
			foreach (AudioClip clip in bgmList) {
				bgmBuffer[clip.name] = clip;
			}
			#endregion

			#region BGMの初期設定
			bgmSource = this.gameObject.AddComponent<AudioSource>();
			bgmSource.loop = true;
			#endregion

			#region SEの初期設定
			//最初にデフォルトのAudioSourceを用意する
			CreateAndEnqueueAudioSource(seSize);
			#endregion

			//シーン遷移で削除されないようにする
			DontDestroyOnLoad(this.gameObject);

		}

		/// <summary>
		/// SE用のAudioSourceを新しく用意する
		/// 用意されたAudioSourceはオブジェクトプールに追加される
		/// </summary>
		/// <param name="size">追加する量</param>
        private void CreateAndEnqueueAudioSource(uint size) {
			for(int i = 0; i < size; i++) {
				AudioSource audioSource = gameObject.AddComponent<AudioSource>();
				audioSource.playOnAwake = false;
				audioSource.enabled = false;
				seSourcePool.Enqueue(audioSource);
			}
		}

		/// <summary>
		/// 使用可能なAudioSourceをプールから取得する
		/// プールから取得できなければ新しく追加する
		/// </summary>
		/// <returns></returns>
		private AudioSource GetAudioSourceFromPool() {
			if (seSourcePool.Count <= 0) {
				Debug.LogWarning("[ToyBox]Audioの許容量を超えたためAudioSourceを追加しました");
				CreateAndEnqueueAudioSource(1);
			} 
			return seSourcePool.Dequeue();
		}

		/// <summary>
		/// 再生するSEを新しく登録する
		/// ※Tag名が既に存在している場合は登録できない
		/// ※指定されたファイル名が見つからない場合も登録できない
		/// </summary>
		/// <param name="tagName">登録名</param>
		/// <param name="clipName">再生するSEファイル名</param>
        public void RegisterSe(string tagName,string clipName) {
			if (!seBuffer.ContainsKey(clipName)) {
				Debug.LogError("[ToyBox]指定されたファイルが見つかりません:" + "<color=red>" + clipName + "</color>");
				return;
			}
			if (activeAudioSources.ContainsKey(tagName)) {
				Debug.LogWarning("[ToyBox]Tag名が重複したため上書きしました:" + "<color=red>" + tagName + "</color>");
				ReleaseSe(tagName);
			}

			AudioSource audioSource = this.GetAudioSourceFromPool();
			audioSource.enabled = true;
			audioSource.clip = seBuffer[clipName];
			activeAudioSources.Add(tagName , audioSource);
		}

		/// <summary>
		/// 登録されたSEを再生する
		/// </summary>
		/// <param name="tagName">登録名</param>
		/// <param name="isLoop">ループするか</param>
        public void PlaySe(string tagName,bool isLoop = false) {
			if (!activeAudioSources.ContainsKey(tagName)) {
				Debug.LogWarning("[ToyBox]指定されたTagで登録されたSEが見つかりません:" + "<color=red>" + tagName + "</color>");
				return;
			}

			AudioSource audioSource = activeAudioSources[tagName];
			audioSource.volume = audioOption.seVolume;
			audioSource.loop = isLoop;
			audioSource.Play();
		}

		/// <summary>
		/// SEの再生を停止させる
		/// </summary>
		/// <param name="tagName">登録名</param>
        public void StopSe(string tagName) {
			if (!activeAudioSources.ContainsKey(tagName)) {
				Debug.LogWarning("[ToyBox]指定されたTagで登録されたSEが見つかりません:" + "<color=red>" + tagName + "</color>");
				return;
			}

			AudioSource audioSource = activeAudioSources[tagName];
			audioSource.Stop();
		}

		/// <summary>
		/// SEを終了させ、登録を破棄する
		/// </summary>
		/// <param name="tagName">登録名</param>
        public void ReleaseSe(string tagName) {
			if (!activeAudioSources.ContainsKey(tagName)) {
				Debug.LogWarning("[ToyBox]指定されたTagで登録されたSEが見つかりません:" + "<color=red>" + tagName + "</color>");
				return;
			}

			AudioSource audioSource = activeAudioSources[tagName];
			audioSource.Stop();
			audioSource.enabled = false;
			activeAudioSources.Remove(tagName);
			seSourcePool.Enqueue(audioSource);
		}

		/// <summary>
		/// SEを登録せずに直接再生させる
		/// ※登録せずに再生させる場合は管理を行わないので注意
		/// </summary>
		/// <param name="clipName">再生するSEファイル名</param>
        public void QuickPlaySe(string clipName) {
			if (!seBuffer.ContainsKey(clipName)) {
				Debug.LogError("[ToyBox]指定されたファイルが見つかりません:" + "<color=red>" + clipName + "</color>");
				return;
			}

			//ユニーク（唯一）な名前にするために現在時刻 + ランダムな数字で登録する
			string seName = clipName + System.DateTime.Now.Hour +
				System.DateTime.Now.Minute + System.DateTime.Now.Second + System.DateTime.Now.Millisecond +
				UnityEngine.Random.Range(0 , 255);

			RegisterSe(seName , clipName);
			StartCoroutine(PlayAndReleaseCoroutine(seName));
		}

		/// <summary>
		/// SEを再生させる
		/// SEの再生が終了したらプールへ戻す
		/// </summary>
		/// <param name="tagName">登録名</param>
		/// <returns></returns>
        private IEnumerator PlayAndReleaseCoroutine(string tagName) {
            
			AudioSource quickAudio = activeAudioSources[tagName];

			PlaySe(tagName);

			//再生終了後、自動で解放をおこなう
			yield return new WaitWhile(() => quickAudio.isPlaying);
			ReleaseSe(tagName);
		}

		/// <summary>
		/// BGMを登録する
		/// </summary>
		/// <param name="clipName">登録するBGMファイル名</param>
        public void RegisterBgm(string clipName) {
			if (!bgmBuffer.ContainsKey(clipName)) {
				Debug.LogError("[ToyBox]指定されたファイルが見つかりません:" + "<color=red>" + clipName + "</color>");
				return;
			}
			bgmSource.clip = bgmBuffer[clipName];
		}
		
		/// <summary>
		/// BGMを再生させる
		/// </summary>
        public void PlayBgm() {
			if (bgmSource.isPlaying) return;
			bgmSource.volume = audioOption.bgmVolume;
			bgmSource.Play();
		}

		/// <summary>
		/// BGMをフェードインで再生させる
		/// </summary>
		/// <param name="fadeDuration">フェードにかける時間</param>
        public void PlayBgm(float fadeDuration) {
			if (bgmSource.isPlaying) return;
			
			if (audioOption.fadeCoroutine == null) {
				audioOption.fadeCoroutine = StartCoroutine(FadeInBgmCoroutine(fadeDuration));
				bgmSource.Play();
			}
		}

		/// <summary>
		/// BGMの再生を終了させる
		/// </summary>
        public void StopBgm() {
			if (!bgmSource.isPlaying) return;
			bgmSource.Stop();
		}
		
		/// <summary>
		/// BGMの再生をフェードアウトで終了させる
		/// </summary>
		/// <param name="fadeDuration">フェードにかける時間</param>
        public void StopBgm(float fadeDuration) {
			if (!bgmSource.isPlaying) return;

			if (audioOption.fadeCoroutine == null) {
				audioOption.fadeCoroutine = StartCoroutine(FadeOutBgmCoroutine(fadeDuration));
			}
		}

		/// <summary>
		/// BGMの再生を一時停止させる
		/// </summary>
        public void PauseBgm() {
			bgmSource.Pause();
		}

		/// <summary>
		/// BGMの再生を再開する
		/// </summary>
        public void ResumeBgm() {
			bgmSource.UnPause();
		}

		/// <summary>
		/// BGMをフェードインさせる
		/// </summary>
		/// <param name="fadeDuration">フェードにかける時間</param>
		/// <returns></returns>
        private IEnumerator FadeInBgmCoroutine(float fadeDuration) {
			float targetVolume = audioOption.bgmVolume;
			bgmSource.volume = 0;
			while (bgmSource.volume < targetVolume) {
				yield return null;
				targetVolume = audioOption.bgmVolume;
				float increase = (targetVolume * Time.deltaTime) / fadeDuration;
				bgmSource.volume += increase;
			}
			bgmSource.volume = targetVolume;
			audioOption.fadeCoroutine = null;
		}
		
		/// <summary>
		/// BGMをフェードアウトさせる
		/// </summary>
		/// <param name="fadeDuration">フェードにかける時間</param>
		/// <returns></returns>
        private IEnumerator FadeOutBgmCoroutine(float fadeDuration) {

			float targetVolume = audioOption.bgmVolume;

			while (bgmSource.volume > 0) {
				yield return null;
				float decrease = (targetVolume * Time.deltaTime) / fadeDuration;
				bgmSource.volume -= decrease;
			}
			bgmSource.volume = 0;
			bgmSource.Stop();
			audioOption.fadeCoroutine = null;
		}

		/// <summary>
		/// SEの音量を設定する
		/// </summary>
		/// <param name="volume">音量</param>
        public void SetSeVolume(float volume) {
			audioOption.seVolume = volume;
			//設定を反映させる
			foreach (AudioSource audioSource in activeAudioSources.Values) {
				audioSource.volume = audioOption.seVolume;
			}
		}

		/// <summary>
		/// BGMの音量を設定する
		/// </summary>
		/// <param name="volume">音量</param>
		/// <param name="isStopFade">フェードを中断するか</param>
        public void SetBgmVolume(float volume,bool isStopFade = false) {
			audioOption.bgmVolume = volume;
			
			if (audioOption.fadeCoroutine != null) {
				if (isStopFade) {
					StopCoroutine(audioOption.fadeCoroutine);
					audioOption.fadeCoroutine = null;
					bgmSource.volume = audioOption.bgmVolume;
				}
			}
			else {
				bgmSource.volume = audioOption.bgmVolume;
			}
		}

		/// <summary>
		/// SEの音量設定
		/// </summary>
        public float SeVolume {
			get {
				return audioOption.seVolume;
			}
			set {
				SetSeVolume(value);
			}
		}

		/// <summary>
		/// BGMの音量設定
		/// </summary>
        public float BgmVolume {
			get {
				return audioOption.bgmVolume;
			}
			set {
				//設定を反映させる
				SetBgmVolume(value);
			}
		}

		#region Inspectorで音量調節を可能にするための処理
#if UNITY_EDITOR

        private float seVolumeBuf = 0;
        private float bgmVolumeBuf = 0;

		void Update() {
			if(seVolumeBuf != audioOption.seVolume) {
				seVolumeBuf = audioOption.seVolume;
				//設定を反映させる
				foreach (AudioSource audioSource in activeAudioSources.Values) {
					audioSource.volume = audioOption.seVolume;
				}
			}
			if (bgmVolumeBuf != audioOption.bgmVolume) {
				bgmVolumeBuf = audioOption.bgmVolume;
				//設定を反映させる
				if (audioOption.fadeCoroutine == null) {
					bgmSource.volume = audioOption.bgmVolume;
				}
			}
		}
#endif
		#endregion
	}
}