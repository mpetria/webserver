using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServer.Entities
{

    public enum ResponseStatusCode
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

    public class ResponseStatusDescription
    {
        public static Dictionary<ResponseStatusCode, string> DefaultDescriptions = new Dictionary<ResponseStatusCode, string>()
                {
                     {ResponseStatusCode.Continue                   	,	"Continue"},
                     {ResponseStatusCode.SwitchingProtocols	        	,	"Switching Protocols"},
					                                                    
                     {ResponseStatusCode.OK                         	,	"OK"},
                     {ResponseStatusCode.Created                    	,	"Created"},
                     {ResponseStatusCode.Accepted                   	,	"Accepted"},
                     {ResponseStatusCode.NonAuthoritativeInformation 	,	"Non-Authoritative Information"},
                     {ResponseStatusCode.NoContent	                 	,	"No Content"},
                     {ResponseStatusCode.ResetContent	              	,	"Reset Content"},
                     {ResponseStatusCode.PartialContent 	           	,	"Partial Content"},
						                                                
                     {ResponseStatusCode.MultipleChoices	           	,	"Multiple Choices"},
                     {ResponseStatusCode.MovedPermanently	          	,	"Moved Permanently"},
                     {ResponseStatusCode.Found                      	,	"Found"},
                     {ResponseStatusCode.SeeOther	                  	,	"See Other"},
                     {ResponseStatusCode.NotModified	               	,	"Not Modified"},
                     {ResponseStatusCode.UseProxy	                  	,	"Use Proxy"},
								                                        
                     {ResponseStatusCode.TemporaryRedirect	         	,	"Temporary Redirect"},
								                                        
                     {ResponseStatusCode.BadRequest	                	,	"Bad Request"},
                     {ResponseStatusCode.Unauthorized               	,	"Unauthorized"},
                     {ResponseStatusCode.PaymentRequired	           	,	"Payment Required"},
                     {ResponseStatusCode.Forbidden                  	,	"Forbidden"},
                     {ResponseStatusCode.NotFound	                  	,	"Not Found"},
                     {ResponseStatusCode.MethodNotAllowed	         	,	"Method Not Allowed"},
                     {ResponseStatusCode.NotAcceptable	             	,	"Not Acceptable"},
                     {ResponseStatusCode.ProxyAuthenticationRequired 	,	"Proxy Authentication Required"},
                     {ResponseStatusCode.RequestTimeout	            	,	"Request Timeout"},
                     {ResponseStatusCode.Conflict                   	,	"Conflict"},
                     {ResponseStatusCode.Gone                       	,	"Gone"},
                     {ResponseStatusCode.LengthRequired	            	,	"Length Required"},
                     {ResponseStatusCode.PreconditionFailed	        	,	"Precondition Failed"},
                     {ResponseStatusCode.RequestEntityTooLarge		   	,	"Request Entity Too Large"},
                     {ResponseStatusCode.RequestURITooLong		       	,	"Request-URI Too Long"},
                     {ResponseStatusCode.UnsupportedMediaType	     	,	"Unsupported Media Type"},
                     {ResponseStatusCode.RequestedRangeNotSatisfiable	,	"Requested Range Not Satisfiable"},
                     {ResponseStatusCode.ExpectationFailed	         	,	"Expectation Failed"},
						                                                
                     {ResponseStatusCode.InternalServerError	      	,	"Internal Server Error"},
                     {ResponseStatusCode.NotImplemented	            	,	"Not Implemented"},
                     {ResponseStatusCode.BadGateway	                	,	"Bad Gateway"},
                     {ResponseStatusCode.ServiceUnavailable	        	,	"Service Unavailable"},
                     {ResponseStatusCode.GatewayTimeout	            	,	"Gateway Timeout"},
                     {ResponseStatusCode.HTTPVersionNotSupported	 	,	"HTTP Version Not Supported"} 
                                    
                };
    }






}
