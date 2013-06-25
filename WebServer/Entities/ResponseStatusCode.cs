using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Entities
{

    public enum ResponseStatusCode
    {
        Continue = 100,

        Ok = 200,
        NoContent = 204,

        NotModified =304,

        BadRequest = 400,
        NotFound = 404,
        MethodNotAllowed = 405,

        InternalServerError = 500,
        
    }

    public class ResponseStatusDescription
    {
        public static Dictionary<ResponseStatusCode, string> DefaultDescriptions = new Dictionary<ResponseStatusCode, string>()
                {
                    { ResponseStatusCode.Ok, "OK" },

                    { ResponseStatusCode.NotModified, "Not Modified" },   

                    { ResponseStatusCode.BadRequest, "Bad Request" },
                    { ResponseStatusCode.NotFound, "Not Found" },
                    { ResponseStatusCode.MethodNotAllowed, "Method Not Allowed"},

                    { ResponseStatusCode.InternalServerError, "Internal Server Error" },
                                    
                };
    }
}
