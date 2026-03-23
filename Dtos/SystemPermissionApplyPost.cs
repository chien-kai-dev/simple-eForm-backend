using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace axiosTest.Dtos
{
    public class SystemPermissionApplyPost
    {
        public string applicantDept { get; set; }
        public string applicantName { get; set; }
        public string applyDate { get; set; }
        public string applyReason { get; set; }
    }
}
