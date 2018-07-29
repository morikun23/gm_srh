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
        public ButtonEventTrigger trigger { get; private set; }

        /// <summary>コールバック</summary>
        public System.Action action { get; private set; }

        /// <summary>引数ありコールバック</summary>
        public System.Action<object> objectAction { get; private set; }

        /// <summary>コールバック時の引数</summary>
        public object callBackValue;

        /// <summary>
        /// コンストラクタ
        /// コールバックを設定する
        /// </summary>
        /// <param name="trigger">タイミング</param>
        /// <param name="action">コールバック</param>
        public ButtonEvent(ButtonEventTrigger trigger, Action action) {
            this.trigger = trigger;
            this.action = action;
        }

        /// <summary>
        /// コンストラクタ
        /// 引数ありコールバックを設定する
        /// </summary>
        /// <param name="trigger">タイミング</param>
        /// <param name="objectAction">コールバック</param>
        /// <param name="value">引数</param>
        public ButtonEvent(ButtonEventTrigger trigger, System.Action<object> objectAction, object value) {
            this.trigger = trigger;
            this.objectAction = objectAction;
            this.callBackValue = value;
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
        [SerializeField,Header("自動スケーリング")]
        private bool isAutoScale = true;

        /// <summary>
        /// スケーリング具合
        /// </summary>
        [SerializeField, Range(0.0f, 1.0f),Header("スケーリング率")]
        private float scalePower = 0.1f;

        [SerializeField, Range(0.0f, 1.0f), Header("スケーリング速度")]
        private float scaleSpeed = 0.25f;

        /// <summary>
        /// 長押しとみなす時間
        /// </summary>
        [SerializeField,Header("長押しとみなす時間")]
        private float longPressTime = 1.5f;

        /// <summary>
        /// 自動でスケーリング処理をおこなう
        /// </summary>
        public bool IsAutoScale {
            get { return isAutoScale; }
        }

        /// <summary>
        /// AutoScaleモードでスケーリングする量
        /// </summary>
        public float ScalePower {
            get { return 1.0f - scalePower; }
        }

        /// <summary>
        /// AutoScaleモードでスケーリングする速度
        /// </summary>
        public float ScaleSpeed {
            get { return 1.0f - scaleSpeed; }
        }

        /// <summary>
        /// 長押しとみなす時間
        /// </summary>
        public float LongPressTime{
            get { return longPressTime; }
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
        protected readonly List<ButtonEvent> btnEvents = new List<ButtonEvent>();

        /// <summary>ボタンの設定</summary>
        [SerializeField]
        protected ButtonOption btnOption;

		/// <summary>長押し検出用コルーチン</summary>
		private Coroutine longPressDetectCoroutine;

		/// <summary>
        /// このボタンが現在押されている状態であるか
        /// </summary>
        public bool IsUsing { get; protected set; }

        /// <summary>
        /// ボタンの設定
        /// ※読み取り専用
        /// </summary>
        public ButtonOption BtnOption {
            get { return btnOption; }
        }

        /// <summary>
        /// コールバックを追加する
        /// </summary>
        /// <param name="btnEvent">コールバック情報</param>
        public void AddButtonEvent(ButtonEvent btnEvent) {
            if(btnEvent == null) {
                Debug.LogWarning("[ToyBox]コールバックがNULL");
            }
            else {
                btnEvents.Add(btnEvent);
            }
        }

		/// <summary>
		/// コールバック情報を初期化する
		/// </summary>
		public void ResetEventCallBack() {
			btnEvents.Clear();
		}
        /// <summary>
        /// カーソルが押されたときの処理
        /// </summary>
        protected virtual void OnPressed() {
            if(btnOption.IsAutoScale) {
                //スケール処理
                this.transform.DOScale(Vector3.one * btnOption.ScalePower, btnOption.ScaleSpeed);
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
            if(btnOption.IsAutoScale) {
                this.transform.DOScale(Vector3.one, btnOption.ScaleSpeed);
            }
        }

        /// <summary>
        /// 長押しとみなすかの検出を行う
        /// </summary>
        private IEnumerator DetectLongPress(){
            yield return new WaitForSeconds(btnOption.LongPressTime);
            if(IsUsing){
                //長押しコールバック実行
                foreach(var action in btnEvents.FindAll(_ => _.trigger == ButtonEventTrigger.OnLongPress)) {
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
 
            if(btnEvent.action != null) {
                btnEvent.action();
                return;
            }

            if(btnEvent.objectAction != null) {
                btnEvent.objectAction(btnEvent.callBackValue);
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

            foreach(var action in btnEvents.FindAll(_ => _.trigger == ButtonEventTrigger.OnClick)) {
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
            this.longPressDetectCoroutine = StartCoroutine(this.DetectLongPress());

            this.OnPressed();

            foreach(var action in btnEvents.FindAll(_ => _.trigger == ButtonEventTrigger.OnDown)) {
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
            StopCoroutine(this.longPressDetectCoroutine);

            this.OnReleased();

            foreach(var action in btnEvents.FindAll(_ => _.trigger == ButtonEventTrigger.OnRelease)) {
                ExecButtonEvent(action);
            }
        }
    }
}