using NeoErp.Transaction.Service.Models;
using NeoErp.Transaction.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.Transaction.Controllers
{
    public class TargetApiController : ApiController
    {
        public ITargetServiceRepository _targetServiceRepository;
        public TargetApiController(ITargetServiceRepository targetServiceRepository)
        {
            this._targetServiceRepository = targetServiceRepository;
        }
        

        [HttpGet]
        public List<Target> GetAllTargetList()
        {
            List<Target> targetList = this._targetServiceRepository.GetAllTargets();
            return targetList;
        }

        [HttpPost]
        public string PostTarget(Target target)
        {
            try
            {
                if (string.IsNullOrEmpty(target.TARGET_CODE))
                {
                    this._targetServiceRepository.AddNewTarget(target);
                }
                else
                {
                    this._targetServiceRepository.UpdateTarget(target);
                }
                return "success";
            }
            catch (Exception)
            {
                return "failed";
            }
            
        }

        [HttpPost]
        public string DeleteTarget(Target target)
        {
            try
            {
                this._targetServiceRepository.DeleteTarget(target);
                return "success";
            }
            catch (Exception ex)
            {
                return "failed";
            }
            
        }
    }
}
