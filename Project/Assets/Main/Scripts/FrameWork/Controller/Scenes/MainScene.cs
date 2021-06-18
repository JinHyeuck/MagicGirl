using GameBerry.Managers;

namespace GameBerry.Scene
{
    // 첫Scene
    // 패치와 클라이언트 선 로드를 해준다.
    public class MainScene : IScene
    {
        protected override void OnLoadComplete()
        {
            UnityEngine.Debug.LogError("메인씬 로드 완료");
            SceneManager.Instance.Load(Constants.SceneName.InGame);
        }
    }
}