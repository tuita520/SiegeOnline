﻿//-----------------------------------------------------------------------------------------------------------
// Copyright (C) 2015-2016 SiegeOnline
// 版权所有
//
// 文件名：Equipment.cs
//
// 文件功能描述：
//
// 装备抽象基类，定义了装备的共用属性及方法
//
// 创建标识：taixihuase 20150816
//
// 修改标识：
// 修改描述：
// 
//
// 修改标识：
// 修改描述：
//
//----------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable InconsistentNaming

namespace SiegeOnlineServer.Protocol.Common.Item.Equipment
{
    /// <summary>
    /// 类型：类
    /// 名称：Equipment
    /// 作者：taixihuase
    /// 作用：装备抽象基类
    /// 编写日期：2015/8/16
    /// </summary>
    [Serializable]
    public class Equipment : IItem
    {
        public int FixedId { get; protected set; } // 数据库固定编号

        public int AllocatedId { get; protected set; } // 动态分配编号

        public string Name { get; protected set; }

        public byte Occupation { get; protected set; }

        public int LevelLimit { get; protected set; }

        public int CurrentLevel { get; set; }

        public int Durability { get; set; }

        #region 装备类型

        [Serializable]
        public enum EquipmentType : byte
        {
            Null,
            Armor,
            Jewel,
            Weapon
        }

        public byte EquipType { get; protected set; }

        #endregion

        public List<KeyValuePair<AttributeCode, float>> FixedAttributes; // 固定属性（多条）
        public KeyValuePair<AttributeCode, float> RandomAttribute; // 随机属性（单条）

        public Equipment(int @fixed, int allocated, string name, OccupationCode occupation, int limit, int cur,
            EquipmentType type)
        {
            FixedId = @fixed;
            AllocatedId = allocated;
            Name = name;
            Occupation = (byte) occupation;
            LevelLimit = limit;
            CurrentLevel = cur;
            EquipType = (byte) type;
            FixedAttributes = new List<KeyValuePair<AttributeCode, float>>
            {
                new KeyValuePair<AttributeCode, float>(AttributeCode.Null, 0)
            };
            RandomAttribute = new KeyValuePair<AttributeCode, float>(AttributeCode.Null, 0);
        }

        public Equipment(Equipment equipment)
        {
            FixedId = equipment.FixedId;
            AllocatedId = equipment.AllocatedId;
            Name = equipment.Name;
            Occupation = equipment.Occupation;
            LevelLimit = equipment.LevelLimit;
            CurrentLevel = equipment.CurrentLevel;
            EquipType = equipment.EquipType;
            FixedAttributes = equipment.FixedAttributes;
            RandomAttribute = equipment.RandomAttribute;
        }

        public Equipment()
        {
            FixedId = 0;
            AllocatedId = 0;
            Name = "";
            Occupation = (byte) OccupationCode.Common;
            LevelLimit = 0;
            CurrentLevel = 0;
            EquipType = (byte) EquipmentType.Null;
            FixedAttributes = new List<KeyValuePair<AttributeCode, float>>
            {
                new KeyValuePair<AttributeCode, float>(AttributeCode.Null, 0)
            };
            RandomAttribute = new KeyValuePair<AttributeCode, float>(AttributeCode.Null, 0);
        }

        public virtual void Apply(Character.Character character)
        {
            foreach (KeyValuePair<AttributeCode, float> fixedAttribute in FixedAttributes)
            {
                UpdateCharacterAttribute(character, fixedAttribute, true);
            }
            CalculateCharacterAttributes(character);
        }

        public virtual void Cancel(Character.Character character)
        {
            foreach (KeyValuePair<AttributeCode, float> fixedAttribute in FixedAttributes)
            {
                UpdateCharacterAttribute(character, fixedAttribute, false);
            }
            CalculateCharacterAttributes(character);
        }

        public void Repair()
        {
            if (Durability < DataConstraint.EquipmentMaxDurability)
            {
                Durability = DataConstraint.EquipmentMaxDurability;
            }
        }

        public void Abrase(int value)
        {
            Durability -= value;
            if (Durability <= 0)
            {
                Durability = 0;
            }
        }

        protected void UpdateCharacterAttribute(Character.Character character,
            KeyValuePair<AttributeCode, float> attribute,
            bool activated)
        {
            int sign = activated ? 1 : -1;
            float v = attribute.Value;
            switch (attribute.Key)
            {
                    #region 采用数组进行存储的属性值改变

                case AttributeCode.Attack_Both:
                    character.Attribute.AttackPhysical[1] += (int) v*sign;
                    character.Attribute.AttackPhysical[2] += (int) v*sign;
                    if (character.Attribute.AttackPhysical[1] <= 0)
                    {
                        character.Attribute.AttackPhysical[1] = 0;
                    }
                    if (character.Attribute.AttackPhysical[2] <= 0)
                    {
                        character.Attribute.AttackPhysical[2] = 0;
                    }
                    character.Attribute.AttackMagic[1] += (int) v*sign;
                    character.Attribute.AttackMagic[2] += (int) v*sign;
                    if (character.Attribute.AttackMagic[1] <= 0)
                    {
                        character.Attribute.AttackMagic[1] = 0;
                    }
                    if (character.Attribute.AttackMagic[2] <= 0)
                    {
                        character.Attribute.AttackMagic[2] = 0;
                    }
                    break;

                case AttributeCode.Attack_Magic:
                    character.Attribute.AttackMagic[1] += (int) v*sign;
                    character.Attribute.AttackMagic[2] += (int) v*sign;
                    if (character.Attribute.AttackMagic[1] <= 0)
                    {
                        character.Attribute.AttackMagic[1] = 0;
                    }
                    if (character.Attribute.AttackMagic[2] <= 0)
                    {
                        character.Attribute.AttackMagic[2] = 0;
                    }
                    break;

                case AttributeCode.Attack_Physical:
                    character.Attribute.AttackPhysical[1] += (int) v*sign;
                    character.Attribute.AttackPhysical[2] += (int) v*sign;
                    if (character.Attribute.AttackPhysical[1] <= 0)
                    {
                        character.Attribute.AttackPhysical[1] = 0;
                    }
                    if (character.Attribute.AttackPhysical[2] <= 0)
                    {
                        character.Attribute.AttackPhysical[2] = 0;
                    }
                    break;

                case AttributeCode.Attack_Percent_Both:
                    character.Attribute.AttackPercentPhysical += v*sign;
                    if (character.Attribute.AttackPercentPhysical <= 0)
                    {
                        character.Attribute.AttackPercentPhysical = 0;
                    }
                    character.Attribute.AttackPercentMagic += v*sign;
                    if (character.Attribute.AttackPercentMagic <= 0)
                    {
                        character.Attribute.AttackPercentMagic = 0;
                    }
                    break;

                case AttributeCode.Attack_Percent_Physical:
                    character.Attribute.AttackPercentPhysical += v*sign;
                    if (character.Attribute.AttackPercentPhysical <= 0)
                    {
                        character.Attribute.AttackPercentPhysical = 0;
                    }
                    break;

                case AttributeCode.Attack_Percent_Magic:
                    character.Attribute.AttackPercentMagic += v*sign;
                    if (character.Attribute.AttackPercentMagic <= 0)
                    {
                        character.Attribute.AttackPercentMagic = 0;
                    }
                    break;

                case AttributeCode.Defense_Both:
                    character.Attribute.DefensePhysical[1] += (int) v*sign;
                    if (character.Attribute.DefensePhysical[1] <= 0)
                    {
                        character.Attribute.DefensePhysical[1] = 0;
                    }
                    character.Attribute.DefenseMagic[1] += (int) v*sign;
                    if (character.Attribute.DefenseMagic[1] <= 0)
                    {
                        character.Attribute.DefenseMagic[1] = 0;
                    }
                    break;

                case AttributeCode.Defense_Physical:
                    character.Attribute.DefensePhysical[1] += (int) v*sign;
                    if (character.Attribute.DefensePhysical[1] <= 0)
                    {
                        character.Attribute.DefensePhysical[1] = 0;
                    }
                    break;

                case AttributeCode.Defense_Magic:
                    character.Attribute.DefenseMagic[1] += (int) v*sign;
                    if (character.Attribute.DefenseMagic[1] <= 0)
                    {
                        character.Attribute.DefenseMagic[1] = 0;
                    }
                    break;

                case AttributeCode.Defense_Percent_Both:
                    character.Attribute.DefensePercentPhysical += v*sign;
                    if (character.Attribute.DefensePercentPhysical <= 0)
                    {
                        character.Attribute.DefensePercentPhysical = 0;
                    }
                    character.Attribute.DefensePercentMagic += v*sign;
                    if (character.Attribute.DefensePercentMagic <= 0)
                    {
                        character.Attribute.DefensePercentMagic = 0;
                    }
                    break;

                case AttributeCode.Defense_Percent_Physical:
                    character.Attribute.DefensePercentPhysical += v*sign;
                    if (character.Attribute.DefensePercentPhysical <= 0)
                    {
                        character.Attribute.DefensePercentPhysical = 0;
                    }
                    break;

                case AttributeCode.Defense_Percent_Magic:
                    character.Attribute.DefensePercentMagic += v*sign;
                    if (character.Attribute.DefensePercentMagic <= 0)
                    {
                        character.Attribute.DefensePercentMagic = 0;
                    }
                    break;

                case AttributeCode.Life_Increase:
                    character.Attribute.HitPoint[1] += (int) v*sign;
                    if (character.Attribute.HitPoint[1] <= 0)
                    {
                        character.Attribute.HitPoint[1] = 0;
                    }
                    break;

                case AttributeCode.Mana_Increase:
                    character.Attribute.Mana[1] += (int) v*sign;
                    if (character.Attribute.Mana[1] <= 0)
                    {
                        character.Attribute.Mana[1] = 0;
                    }
                    break;

                    #endregion

                    #region 根据枚举名反射改变属性值

                default:
                {
                    string code = attribute.Key.ToString().Replace("_", "");
                    Type type = character.Attribute.GetType();
                    if (code.Contains("Both"))
                    {
                        string physical = code.Replace("Both", "Physical");
                        var pi = type.GetProperty(physical);
                        float pv = (float) pi.GetValue(character.Attribute, null);
                        pv += v*sign;
                        if (pv <= 0)
                        {
                            pv = 0;
                        }
                        pi.SetValue(character.Attribute, pv, null);

                        string magic = code.Replace("Both", "Magic");
                        pi = type.GetProperty(magic);
                        float mv = (float) pi.GetValue(character.Attribute, null);
                        mv += v*sign;
                        if (mv <= 0)
                        {
                            mv = 0;
                        }
                        pi.SetValue(character.Attribute, mv, null);
                    }
                    else if (code.Contains("All"))
                    {
                        string attr = code.Replace("All", "");
                        var pi = type.GetProperties().Where(x => x.Name.Contains(attr));
                        foreach (var propertyInfo in pi)
                        {
                            int newv = (int) propertyInfo.GetValue(character.Attribute, null);
                            newv += (int) v*sign;
                            if (newv <= 0)
                            {
                                newv = 0;
                            }
                            propertyInfo.SetValue(character.Attribute, newv, null);
                        }
                    }
                    else
                    {
                        var pi = type.GetProperty(code);
                        Type t = pi.PropertyType;
                        if (t == typeof (int))
                        {
                            int newv = (int) pi.GetValue(character.Attribute, null);
                            newv += (int) v*sign;
                            if (newv <= 0)
                            {
                                newv = 0;
                            }
                            pi.SetValue(character.Attribute, newv, null);
                        }
                        else if (t == typeof (float))
                        {
                            float newv = (float) pi.GetValue(character.Attribute, null);
                            newv += v*sign;
                            if (newv <= 0)
                            {
                                newv = 0;
                            }
                            pi.SetValue(character.Attribute, newv, null);
                        }
                    }
                    break;
                }

                    #endregion
            }
        }

        protected void CalculateCharacterAttributes(Character.Character character)
        {
            character.Attribute.AttackPhysical[0] = (int)
                ((character.Attribute.AttackPhysical[1] + character.Attribute.AttackPhysical[2])*
                 (100 + character.Attribute.AttackPercentPhysical)*0.005);
            character.Attribute.AttackMagic[0] =
                (int) ((character.Attribute.AttackMagic[1] + character.Attribute.AttackMagic[2])*
                       (100 + character.Attribute.AttackPercentMagic)*0.005);
            character.Attribute.DefensePhysical[0] =
                (int) (character.Attribute.DefensePhysical[1]*
                       (100 + character.Attribute.DefensePercentPhysical)*0.01);
            character.Attribute.DefenseMagic[0] =
                (int) (character.Attribute.DefenseMagic[1]*
                       (100 + character.Attribute.DefensePercentMagic)*0.01);
            character.Attribute.HitPoint[0] =
                (int) (character.Attribute.HitPoint[1]*
                       (100 + character.Attribute.LifeIncreasePercent)*0.01);
            character.Attribute.Mana[0] =
                (int) (character.Attribute.Mana[1]*
                       (100 + character.Attribute.ManaIncreasePercent)*0.01);
            character.Attribute.AttackSpeed += character.Attribute.SpeedAttack;
            character.Attribute.SkillCooldownSpeed = DataConstraint.CharacterDefaultSkillCooldownSpeed -
                                                     character.Attribute.SpeedCooldown;
            character.Attribute.MovementSpeed = DataConstraint.CharacterDefaultMovementSpeed +
                                                character.Attribute.SpeedMovement;
            character.Attribute.ExperienceGainSpeed = DataConstraint.CharacterDefaultExperienceGainSpeed +
                                                      character.Attribute.SpeedExperience;
        }
    }
}