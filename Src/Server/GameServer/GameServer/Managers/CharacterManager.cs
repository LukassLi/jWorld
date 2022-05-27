using Common;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    // 保存所有角色信息
    // add,remove,clear角色信息
    class CharacterManager: Singleton<CharacterManager>
    {
        public Dictionary<int, Character> Characters = new Dictionary<int, Character>();

        public CharacterManager()
        {

        }

        public void Dispose()
        {

        }

        public void Init() { 
        
        }

        public void Clear()
        {
            Characters.Clear();
        }

        public Character AddCharacter(TCharacter cha)
        {
            Character character = new Character(CharacterType.Player,cha);
            Characters.Add(cha.ID, character);
            return character;
        }

        public void Remove(int characterId)
        {
            Characters.Remove(characterId);
        }
    }
}
