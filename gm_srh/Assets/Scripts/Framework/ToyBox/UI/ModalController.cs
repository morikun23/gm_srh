﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ToyBox {
    /// <summary>
    /// モーダル生成クラス
    /// </summary>
	public abstract class ModalController : MonoBehaviour {

        /// <summary>デフォルトの閉じるボタン(OKボタンやバツボタンなど)</summary>
        [SerializeField,Header("閉じるボタン(あれば)")]
        private UIButton _defaultExitButton;

        /// <summary>アニメーション用</summary>
        [SerializeField,Header("モーダル本体のRectTransform")]
        protected RectTransform _modalObject;

        /// <summary>デフォルトのモーダルが閉じた際の引数なしコールバック</summary>
        private System.Action _defaultCallBack;

        /// <summary>デフォルトのモーダルが閉じた際の引数ありコールバック</summary>
        private System.Action<object> _defaultObjectCallBack;

        /// <summary>デフォルトコールバック実行時の引数</summary>
        private object _callBackValue;

        /// <summary>モーダルの開閉速度</summary>
        private const float MODAL_SPEED = 0.25f;

        /// <summary>
        /// アニメーション用モーダル
        /// </summary>
        /// <value>The modal object.</value>
        public RectTransform ModalObject {
			get {
				return _modalObject;
			}
		}

        /// <summary>
        /// モーダルを表示する
        /// </summary>
        public virtual void Show(){
            ModalObject.localScale = Vector3.zero;

            if(_defaultExitButton != null) {
                _defaultExitButton.AddButtonEvent(
                    new ButtonEvent(ButtonEventTrigger.OnClick, this.Hide)
                );
            }

            this.OnBeforeOpen();

            this.gameObject.SetActive(true);

            ModalObject.DOScale(
                Vector3.one,
                MODAL_SPEED
            ).OnComplete(() => this.OnAfterOpen()).Play();
        }

        /// <summary>
        /// モーダルを表示する
        /// </summary>
        /// <param name="defaultCallBack">モーダル表示終了後のコールバック</param>
        public virtual void Show(System.Action defaultCallBack){
            this._defaultCallBack = defaultCallBack;
            this.Show();
        }

		/// <summary>
		/// モーダルを表示する
		/// </summary>
        /// <param name="defaultCallBack">モーダル表示終了後のコールバック</param>
        /// <param name="value">コールバックの引数</param>
        public void Show(System.Action<object> defaultCallBack,object value) {
            this._defaultObjectCallBack = defaultCallBack;
            this._callBackValue = value;
            this.Show();
		}

        /// <summary>
        /// モーダルを閉じる
        /// </summary>
        public virtual void Hide() {

            OnBeforeClose();

            ModalObject.DOScale(
                Vector3.zero,
                MODAL_SPEED
            ).OnComplete(() => this.OnRemoved()).Play();
        }

        /// <summary>
        /// モーダル表示終了時の処理
        /// コールバックが設定されていれば実行
        /// </summary>
        protected void OnRemoved() {
            this.OnAfterClose();

            if(_defaultCallBack != null){
                _defaultCallBack();
            }

            if(_defaultObjectCallBack != null){
                _defaultObjectCallBack(_callBackValue); 
            }

            Destroy(this.gameObject);
        }

        /// <summary>
        /// モーダルが開き始めるときに実行する
        /// </summary>
        protected virtual void OnBeforeOpen() { }

		/// <summary>
		/// モーダルが開き終わったときに実行する
		/// </summary>
        protected virtual void OnAfterOpen(){ }

        /// <summary>
        /// モーダルが閉じ始めるときに実行する
        /// </summary>
        protected virtual void OnBeforeClose() { }

		/// <summary>
		/// モーダルが閉じ終わったときに実行する
		/// </summary>
        protected virtual void OnAfterClose() { }


    }
}