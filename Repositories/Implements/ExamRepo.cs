﻿using Repositories.Interfaces;
using Repositories.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implements
{
    public class ExamRepo : GenericRepo<Exam>, IExamRepo
    {
        public ExamRepo(ExagenContext context) : base(context)
        {
        }
        public void UpdateOnlyStatus(Exam entity)
        {
            var existingEntity = _dbSet.Find(entity.ExamId);
            if (existingEntity != null)
            {
                existingEntity.Status = entity.Status;
            }
        }

    }
}
