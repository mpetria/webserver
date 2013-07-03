using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Data
{

    public enum HttpStatusCode
    {
         Continue                   	= 100,
         SwitchingProtocols	        	= 101,
										
         OK                         	= 200,
         Created                    	= 201,
         Accepted                   	= 202,
         NonAuthoritativeInformation 	= 203,
         NoContent	                 	= 204,
         ResetContent	              	= 205,
         PartialContent 	           	= 206,
										
         MultipleChoices	           	= 300,
         MovedPermanently	          	= 301,
         Found                      	= 302,
         SeeOther	                  	= 303,
         NotModified	               	= 304,
         UseProxy	                  	= 305,
										
         TemporaryRedirect	         	= 307,
										
         BadRequest	                	= 400,
         Unauthorized               	= 401,
         PaymentRequired	           	= 402,
         Forbidden                  	= 403,
         NotFound	                  	= 404,
         MethodNotAllowed	         	= 405,
         NotAcceptable	             	= 406,
         ProxyAuthenticationRequired 	= 407, 
         RequestTimeout	            	= 408,
         Conflict                   	= 409,
         Gone                       	= 410,
         LengthRequired	            	= 411,
         PreconditionFailed	        	= 412,
         RequestEntityTooLarge		   	= 413,
         RequestURITooLong		       	= 414,
         UnsupportedMediaType	     	= 415,
         RequestedRangeNotSatisfiable	= 416,
         ExpectationFailed	         	= 417,
										
         InternalServerError	      	= 500,
         NotImplemented	            	= 501,
         BadGateway	                	= 502,
         ServiceUnavailable	        	= 503,
         GatewayTimeout	            	= 504,
         HTTPVersionNotSupported	 	= 505, 
        
    }

    public class HttpStatusDescription
    {
        public static Dictionary<HttpStatusCode, string> DefaultDescriptions = new Dictionary<HttpStatusCode, string>()
                {
                     {HttpStatusCode.Continue                   	,	"Continue"},
                     {HttpStatusCode.SwitchingProtocols	        	,	"Switching Protocols"},
					                                                    
                     {HttpStatusCode.OK                         	,	"OK"},
                     {HttpStatusCode.Created                    	,	"Created"},
                     {HttpStatusCode.Accepted                   	,	"Accepted"},
                     {HttpStatusCode.NonAuthoritativeInformation 	,	"Non-Authoritative Information"},
                     {HttpStatusCode.NoContent	                 	,	"No Content"},
                     {HttpStatusCode.ResetContent	              	,	"Reset Content"},
                     {HttpStatusCode.PartialContent 	           	,	"Partial Content"},
						                                                
                     {HttpStatusCode.MultipleChoices	           	,	"Multiple Choices"},
                     {HttpStatusCode.MovedPermanently	          	,	"Moved Permanently"},
                     {HttpStatusCode.Found                      	,	"Found"},
                     {HttpStatusCode.SeeOther	                  	,	"See Other"},
                     {HttpStatusCode.NotModified	               	,	"Not Modified"},
                     {HttpStatusCode.UseProxy	                  	,	"Use Proxy"},
								                                        
                     {HttpStatusCode.TemporaryRedirect	         	,	"Temporary Redirect"},
								                                        
                     {HttpStatusCode.BadRequest	                	,	"Bad Request"},
                     {HttpStatusCode.Unauthorized               	,	"Unauthorized"},
                     {HttpStatusCode.PaymentRequired	           	,	"Payment Required"},
                     {HttpStatusCode.Forbidden                  	,	"Forbidden"},
                     {HttpStatusCode.NotFound	                  	,	"Not Found"},
                     {HttpStatusCode.MethodNotAllowed	         	,	"Method Not Allowed"},
                     {HttpStatusCode.NotAcceptable	             	,	"Not Acceptable"},
                     {HttpStatusCode.ProxyAuthenticationRequired 	,	"Proxy Authentication Required"},
                     {HttpStatusCode.RequestTimeout	            	,	"Request Timeout"},
                     {HttpStatusCode.Conflict                   	,	"Conflict"},
                     {HttpStatusCode.Gone                       	,	"Gone"},
                     {HttpStatusCode.LengthRequired	            	,	"Length Required"},
                     {HttpStatusCode.PreconditionFailed	        	,	"Precondition Failed"},
                     {HttpStatusCode.RequestEntityTooLarge		   	,	"Request Entity Too Large"},
                     {HttpStatusCode.RequestURITooLong		       	,	"Request-URI Too Long"},
                     {HttpStatusCode.UnsupportedMediaType	     	,	"Unsupported Media Type"},
                     {HttpStatusCode.RequestedRangeNotSatisfiable	,	"Requested Range Not Satisfiable"},
                     {HttpStatusCode.ExpectationFailed	         	,	"Expectation Failed"},
						                                                
                     {HttpStatusCode.InternalServerError	      	,	"Internal Server Error"},
                     {HttpStatusCode.NotImplemented	            	,	"Not Implemented"},
                     {HttpStatusCode.BadGateway	                	,	"Bad Gateway"},
                     {HttpStatusCode.ServiceUnavailable	        	,	"Service Unavailable"},
                     {HttpStatusCode.GatewayTimeout	            	,	"Gateway Timeout"},
                     {HttpStatusCode.HTTPVersionNotSupported	 	,	"HTTP Version Not Supported"} 
                                    
                };
    }


    public static class HttpStatusCodeExtensions
    {
        public static bool IsSuccessCode(this HttpStatusCode code)
        {
            return code == HttpStatusCode.OK || code == HttpStatusCode.Created;
        }
    }






}
