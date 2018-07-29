using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using DG.Tweening;

namespace ToyBox {

    /// <summary>コールバックを実行するタイミング</summary>
    public enum ButtonEventTrigger {
        OnClick,
        OnDown,
        OnRelease,
        OnLongPress,
    }

    /// <summary>
    /// ボタンのコールバック（通知）機能
    /// </summary>
    public class ButtonEvent {

        /// <summary>押されたとみなすタイミング</summary>
        public ButtonEventTrigger EventTrigger { get; private set; }

        /// <summary>コールバック</summary>
        public System.Action EventCallBack { get; private set; }

        /// <summary>引数ありコールバック</summary>
        public System.Action<object> EventObjectCallBack { get; private set; }

        /// <summary>コールバック時の引数</summary>
        public object CallBackValue { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// コールバックを設定する
        /// </summary>
        /// <param name="trigger">タイミング</param>
        /// <param name="action">コールバック</param>
        public ButtonEvent(ButtonEventTrigger trigger, Action action) {
            this.EventTrigger = trigger;
            this.EventCallBack = action;
        }

        /// <summary>
        /// コンストラクタ
        /// 引数ありコールバックを設定する
        /// </summary>
        /// <param name="trigger">タイミング</param>
        /// <param name="objectAction">コールバック</param>
        /// <param name="value">引数</param>
        public ButtonEvent(ButtonEventTrigger trigger, System.Action<object> objectAction, object value) {
            this.EventTrigger = trigger;
            this.EventObjectCallBack = objectAction;
            this.CallBackValue = value;
        }
    }

	/// <summary>
	/// ボタンの設定
	/// ボタンが処理をおこなうときの設定がまとまっている
	/// </summary>
	[System.Serializable]
	public class ButtonOption {

		/// <summary>
		/// ボタンが押されたときに自動でスケーリング処理をするか
		/// ここをfalseにすることで独自のアニメーションを実装可能
		/// </summary>
		[SerializeField, Header("自動スケーリング")]
		private bool _isAutoScale = true;

		/// <summary>
		/// スケーリング具合
		/// </summary>
		[SerializeField, Range(0.0f, 1.0f), Header("スケーリング率")]
		private float _scalePower = 0.1f;

		[SerializeField, Range(0.0f, 1.0f), Header("スケーリング速度")]
		private float _scaleSpeed = 0.5f;

		/// <summary>
		/// 長押しとみなす時間
		/// </summary>
		[SerializeField, Header("長押しとみなす時間")]
		private float _longPressTime = 1.5f;

		/// <summary>
		/// 自動でスケーリング処理をおこなう
		/// </summary>
		public bool IsAutoScale {
			get { return _isAutoScale; }
		}

		/// <summary>
		/// AutoScaleモードでスケーリングする量
		/// </summary>
		public float ScalePower {
			get { return 1.0f - _scalePower; }
		}

		/// <summary>
		/// AutoScaleモードでスケーリングする速度
		/// </summary>
		public float ScaleSpeed {
			get { return 1.0f - _scaleSpeed; }
		}

		/// <summary>
		/// 長押しとみなす時間
		/// </summary>
		public float LongPressTime {
			get { return _longPressTime; }
		}
	}

    /// <summary>
    /// UIに使用されるボタンの機能
    /// </summary>
    [AddComponentMenu("ToyBox/UI/Button")]
    public class UIButton : MonoBehaviour,
                            IPointerClickHandler,
                            IPointerDownHandler,
                            IPointerUpHandler {
        
        /// <summary>ボタンのコールバック情報</summary>
        protected readonly List<ButtonEvent> _btnEvents = new List<ButtonEvent>();

        /// <summary>ボタンの設定</summary>
        [SerializeField]
        protected ButtonOption _btnOption;

		/// <summary>長押し検出用コルーチン</summary>
		private Coroutine _longPressDetectCoroutine;

		/// <summary>
        /// このボタンが現在押されている状態であるか
        /// </summary>
        public bool IsUsing { get; protected set; }

        /// <summary>
        /// ボタンの設定
        /// ※読み取り専用
        /// </summary>
        public ButtonOption BtnOption {
            get { return _btnOption; }
        }

        /// <summary>
        /// コールバックを追加する
        /// </summary>
        /// <param name="btnEvent">コールバック情報</param>
        public void AddButtonEvent(ButtonEvent btnEvent) {
            if(btnEvent == null) {
                AppDebug.LogWarning("[ToyBox]コールバックがNULL");
            }
            else {
                _btnEvents.Add(btnEvent);
            }
        }

		/// <summary>
		/// コールバック情報を初期化する
		/// </summary>
		public void ResetEventCallBack() {
			_btnEvents.Clear();
		}
        /// <summary>
        /// カーソルが押されたときの処理
        /// </summary>
        protected virtual void OnPressed() {
            if(_btnOption.IsAutoScale) {
                //スケール処理
                this.transform.DOScale(Vector3.one * _btnOption.ScalePower, _btnOption.ScaleSpeed);
            }
        }

        /// <summary>
        /// カーソルが押されている間の処理
        /// </summary>
        protected virtual void OnLongPressed() {
        
        }

        /// <summary>
        /// カーソルが離された時の処理
        /// </summary>
        protected virtual void OnReleased() {
            if(_btnOption.IsAutoScale) {
                this.transform.DOScale(Vector3.one, _btnOption.ScaleSpeed);
            }
        }

        /// <summary>
        /// 長押しとみなすかの検出を行う
        /// </summary>
        private IEnumerator DetectLongPress(){
            yield return new WaitForSeconds(_btnOption.LongPressTime);
            if(IsUsing){
                //長押しコールバック実行
                foreach(var action in _btnEvents.FindAll(_ => _.EventTrigger == ButtonEventTrigger.OnLongPress)) {
                    ExecButtonEvent(action);
                }
            }
        }


        /// <summary>
        /// ボタンが押されたときの処理
        /// コールバックを実行する
        /// </summary>
        protected void ExecButtonEvent(ButtonEvent btnEvent) {
            
            if(btnEvent == null) return;
 
            if(btnEvent.EventCallBack != null) {
                btnEvent.EventCallBack();
                return;
            }

            if(btnEvent.EventObjectCallBack != null) {
                btnEvent.EventObjectCallBack(btnEvent.CallBackValue);
                return;
            }
        }


        //=========================================
        // インターフェイスの実装
        //=========================================

        /// <summary>
        /// ボタンがクリックされたときの処理
        /// </summary>
        /// <param name="eventData">Event data.</param>
        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            
            IsUsing = false;

            this.OnReleased();

            foreach(var action in _btnEvents.FindAll(_ => _.EventTrigger == ButtonEventTrigger.OnClick)) {
                ExecButtonEvent(action);
            }
        }

        /// <summary>
        /// ボタンが押された瞬間の処理
        /// </summary>
        /// <param name="eventData">Event data.</param>
        void IPointerDownHandler.OnPointerDown(PointerEventData eventData) {
            
            IsUsing = true;

            //長押しの検出を開始する
            this._longPressDetectCoroutine = StartCoroutine(this.DetectLongPress());

            this.OnPressed();

            foreach(var action in _btnEvents.FindAll(_ => _.EventTrigger == ButtonEventTrigger.OnDown)) {
                ExecButtonEvent(action);
            }
        }

        /// <summary>
        /// ボタンから離した瞬間の処理
        /// </summary>
        /// <param name="eventData">Event data.</param>
        void IPointerUpHandler.OnPointerUp(PointerEventData eventData) {
            
            IsUsing = false;

            //長押しの検出を中断する
            StopCoroutine(this._longPressDetectCoroutine);

            this.OnReleased();

            foreach(var action in _btnEvents.FindAll(_ => _.EventTrigger == ButtonEventTrigger.OnRelease)) {
                ExecButtonEvent(action);
            }
        }
    }
}