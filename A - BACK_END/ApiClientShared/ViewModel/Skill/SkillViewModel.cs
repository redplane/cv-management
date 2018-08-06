﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiClientShared.ViewModel.Skill
{
    public class SkillViewModel
    {
        #region Properties

        public int Id { get; set; }

        public string Name { get; set; }

        public double CreatedTime { get; set; }

        public double? LastModifiedTime { get; set; }

        #endregion
    }
}
