using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    #region Skill

    [Serializable]
    public class SKill
    {
        public int id;
        public string name;
        public float cooldown;
        public int damage;
        public SkillType skillType;
        public ProjectileInfo projectile;
    }

    public class ProjectileInfo
    {
        public string name;
        public float speed;
        public int range;
        public string prefab;
    }

    [Serializable]
    public class SkillData : ILoader<int, SKill>
    {
        public List<SKill> skills = new List<SKill>();

        public Dictionary<int, SKill> MakeDict()
        {
            Dictionary<int, SKill> dict = new Dictionary<int, SKill>();

            foreach (SKill skill in skills)
                dict.Add(skill.id, skill);

            return dict;
        }
    }
    #endregion
}