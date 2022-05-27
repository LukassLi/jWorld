using System;


namespace Models
{
    // 一个单例类
    // 存储和修改用户信息，包括用户，角色，地图等
    public class User: Singleton<User>
    {
        // todo 为什么要进行这样的访问操作？
        private SkillBridge.Message.NUserInfo userInfo; // todo N指的是net吗？
        public SkillBridge.Message.NUserInfo Info
        {
            get { return userInfo; }
        }
        public void SetupUserInfo(SkillBridge.Message.NUserInfo info)
        {
            this.userInfo = info;
        }

        public SkillBridge.Message.NCharacterInfo CurrentCharacter { get; set; }

    }

}
