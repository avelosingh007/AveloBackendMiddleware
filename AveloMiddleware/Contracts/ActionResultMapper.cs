using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AveloMiddleware.Contracts
{
    public class ActionResultMapper : ObjectResult
    {
        public ActionResultMapper(object value, int statusCode)
           : base(value)
        {
            base.StatusCode = statusCode;
        }
    }
}
