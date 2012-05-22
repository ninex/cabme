using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Web;
using cabme.web.Service.Entities;

namespace cabme.web.Service
{

    [ServiceContract(Namespace = "http://cabme.co.za")]
    public interface ICabmeService
    {
        [WebInvoke(Method = "GET",
           UriTemplate = "taxis",
           RequestFormat = WebMessageFormat.Json,
           ResponseFormat = WebMessageFormat.Json,
           BodyStyle = WebMessageBodyStyle.Bare)]
        [OperationContract]
        Taxis GetAllTaxis();
    }
}