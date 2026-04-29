using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using QFramework;

namespace QFramework.GFW
{
	public class LoginPanelData : UIPanelData
	{
		public Sequence blinkSequence;
		public float fadeDuration = 0.5f;
	}
	public partial class LoginPanel : UIPanel
	{
		// 缓存 Model 引用
		private IGFWDemoModel mModel;
		private ResLoader mResLoader = ResLoader.Allocate();
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as LoginPanelData ?? new LoginPanelData();
			// 核心：获取 Model 引用
			mModel = GFWDemo.Interface.GetModel<IGFWDemoModel>();
			btnImage.raycastTarget = false;
			mData.blinkSequence = DOTween.Sequence();
			StartBreathingEffect(mData.blinkSequence);
			inside.Hide();
			LoginButton.onClick.AddListener(() =>
			{
				// 立即禁用按钮，防止动画期间重复点击
				LoginButton.interactable = false;
				FadeOutAndHide(LoginButton.gameObject);
				FadeInAndShow(inside.gameObject);
			});
			SelectButton.onClick.AddListener(() =>
			{
				UIKit.OpenPanel("ServerPanel", UILevel.PopUI);
			});
			StartGame.onClick.AddListener(() =>
			{
				DOTween.KillAll();
				// 异步加载
				GFWDemo.Interface.SendCommand(new LoadSceneCommand("Game"));
			} );
			// 监听当前选中的服务器改变
			mModel.CurrentSelectedServer.RegisterWithInitValue(serverInfo =>
			{
				if (serverInfo != null)
				{
					// 更新当前大区显示文本
					CurrentServerText.text = serverInfo.Type.GetDescription()+"——"+serverInfo.id;
                
				}
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
		}
		
		protected override void OnOpen(IUIData uiData = null)
		{
		}
		
		protected override void OnShow()
		{
		}
		
		protected override void OnHide()
		{
		}
		
		protected override void OnClose()
		{
		}
		
		void StartBreathingEffect(Sequence seq)
		{
			Vector3 originalScale = btnImage.transform.localScale;
    
			// 先停止所有正在进行的动画
			btnImage.DOKill();
    
			// 清空序列（如果是新创建的序列，这步可以省略）
			seq.Kill();
			seq = DOTween.Sequence();
    
			// 方案1：透明度+缩放同步变化的呼吸效果
			// 阶段1：淡出 + 轻微放大
			seq.Append(btnImage.DOFade(0.4f, 1.0f));  // 透明度从1到0.4
			seq.Join(btnImage.transform.DOScale(originalScale * 0.95f, 1.5f)); // 同时放大5%
    
			// 阶段2：淡入 + 恢复缩放
			seq.Append(btnImage.DOFade(1.4f, 1.0f));  // 透明度从0.4回到1
			seq.Join(btnImage.transform.DOScale(originalScale, 1.5f)); // 同时恢复原始大小
    
			// 可选的间隔暂停
			// seq.AppendInterval(0.3f);
    
			seq.SetLoops(-1, LoopType.Yoyo);
			seq.SetEase(Ease.InOutSine);
		}
		
		private void FadeOutAndHide(GameObject targetObject)
		{
			// 1. 获取或添加 CanvasGroup
			CanvasGroup canvasGroup = targetObject.GetComponent<CanvasGroup>();
			if (canvasGroup == null) {
				canvasGroup = targetObject.gameObject.AddComponent<CanvasGroup>();
			}



			// 2. 执行渐隐动画，并在完成后隐藏
			canvasGroup.DOFade(0, mData.fadeDuration)           // 透明度从当前值渐变到0
				.SetEase(Ease.OutQuad)            // 使用“缓出”曲线，结束时更平滑
				.OnComplete(() => {               // 动画完成后的回调
					targetObject.gameObject.SetActive(false); // 真正隐藏
					// loginButton.gameObject.SetActive(false); // 或者您可以选择销毁
					// Destroy(loginButton.gameObject);
				});
		}

		private void FadeInAndShow(GameObject targetObject)
		{
			// 1. 获取或添加 CanvasGroup
			CanvasGroup canvasGroup = targetObject.GetComponent<CanvasGroup>();
			if (canvasGroup == null) {
				canvasGroup = targetObject.gameObject.AddComponent<CanvasGroup>();
			}

			// 2. 立即禁用按钮，防止动画期间重复点击
			LoginButton.interactable = false;

			// 3. 执行渐隐动画，并在完成后隐藏
			canvasGroup.DOFade(1, mData.fadeDuration)           // 透明度从当前值渐变到0
				.SetEase(Ease.OutQuad)            // 使用“缓出”曲线，结束时更平滑
				.OnComplete(() => {               // 动画完成后的回调
					targetObject.gameObject.SetActive(true); // 真正隐藏
					// loginButton.gameObject.SetActive(false); // 或者您可以选择销毁
					// Destroy(loginButton.gameObject);
				});
		}
		protected override void OnDestroy()
		{
			base.OnDestroy();
			// // 1. 彻底杀掉所有动画，防止跳转场景报错
			// transform.DOKill();
			// mData.blinkSequence.Kill();
			// 2. 回收资源
			if (mResLoader != null)
			{
				mResLoader.Recycle2Cache();
				mResLoader = null;
			}
		}
	}
}
