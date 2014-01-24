using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Baro.CoreLibrary.W3
{
    public static class W3Utils
    {
        public static void PostJson(HttpContext context, string json)
        {
            string callback = context.Request.QueryString["callback"];

            if (string.IsNullOrEmpty(callback))
            {
                context.Response.Write(json);
            }
            else
            {
                context.Response.Write(callback);
                context.Response.Write("(");
                context.Response.Write(json);
                context.Response.Write(");");
            }
        }
        public static void SetContextJsonResponse(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Charset = "utf-8";
        }
    }
}
