//WARNING: DON'T EDIT THIS FILE!!!
using Common;

namespace Network
{
    public class MessageDispatch<T> : Singleton<MessageDispatch<T>>
    {
        public void Dispatch(T sender, SkillBridge.Message.NetMessageResponse message)
        {
            if (message.userLogin != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.userLogin); }
            if (message.userRegister != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.userRegister); }
        }

        public void Dispatch(T sender, SkillBridge.Message.NetMessageRequest message)
        {
            if (message.userLogin != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.userLogin); }
            if (message.userRegister != null) { MessageDistributer<T>.Instance.RaiseEvent(sender, message.userLogin); } // todo 为什么泛型可以定义在这里
        }
    }
}