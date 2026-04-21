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
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as LoginPanelData ?? new LoginPanelData();
			// please add init code here
			btnImage.raycastTarget = false;
			mData.blinkSequence = DOTween.Sequence();
			StartBreathingEffect(mData.blinkSequence);
			LoginButton.onClick.AddListener(() =>
			{
				FadeOutAndHideButton();
				//LoginButton.Hide();
			});
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
		
		private void FadeOutAndHideButton()
		{
			// 1. 获取或添加 CanvasGroup
			CanvasGroup canvasGroup = LoginButton.GetComponent<CanvasGroup>();
			if (canvasGroup == null) {
				canvasGroup = LoginButton.gameObject.AddComponent<CanvasGroup>();
			}

			// 2. 立即禁用按钮，防止动画期间重复点击
			LoginButton.interactable = false;

			// 3. 执行渐隐动画，并在完成后隐藏
			canvasGroup.DOFade(0, mData.fadeDuration)           // 透明度从当前值渐变到0
				.SetEase(Ease.OutQuad)            // 使用“缓出”曲线，结束时更平滑
				.OnComplete(() => {               // 动画完成后的回调
					LoginButton.gameObject.SetActive(false); // 真正隐藏
					// loginButton.gameObject.SetActive(false); // 或者您可以选择销毁
					// Destroy(loginButton.gameObject);
				});
		}
	}
}
