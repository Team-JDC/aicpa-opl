#region

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;

#endregion

/// <remarks />
[DebuggerStepThrough]
[DesignerCategory("code")]
[WebServiceBinding(Name = "DataIndexingSoapBinding", Namespace = "urn:com.endeca.service.dataindexing")]
[SoapInclude(typeof (PVal))]
[SoapInclude(typeof (Record))]
[SoapInclude(typeof (SystemError))]
public class DataIndexingService : SoapHttpClientProtocol
{
    /// <remarks />
    public DataIndexingService()
    {
        Url = "http://localhost:8888/services/DataIndexing";
    }

    /// <remarks />
    [SoapRpcMethod("", RequestNamespace = "urn:com.endeca.service.dataindexing",
        ResponseNamespace = "urn:com.endeca.service.dataindexing")]
    [return: SoapElement("getStatusReturn")]
    public Status getStatus()
    {
        object[] results = Invoke("getStatus", new object[0]);
        return ((Status) (results[0]));
    }

    /// <remarks />
    public IAsyncResult BegingetStatus(AsyncCallback callback, object asyncState)
    {
        return BeginInvoke("getStatus", new object[0], callback, asyncState);
    }

    /// <remarks />
    public Status EndgetStatus(IAsyncResult asyncResult)
    {
        object[] results = EndInvoke(asyncResult);
        return ((Status) (results[0]));
    }

    /// <remarks />
    [SoapRpcMethod("", RequestNamespace = "urn:com.endeca.service.dataindexing",
        ResponseNamespace = "urn:com.endeca.service.dataindexing")]
    public void addContent(string in0, Record[] in1)
    {
        Invoke("addContent", new object[]
                                 {
                                     in0,
                                     in1
                                 });
    }

    /// <remarks />
    public IAsyncResult BeginaddContent(string in0, Record[] in1, AsyncCallback callback, object asyncState)
    {
        return BeginInvoke("addContent", new object[]
                                             {
                                                 in0,
                                                 in1
                                             }, callback, asyncState);
    }

    /// <remarks />
    public void EndaddContent(IAsyncResult asyncResult)
    {
        EndInvoke(asyncResult);
    }

    /// <remarks />
    [SoapRpcMethod("", RequestNamespace = "urn:com.endeca.service.dataindexing",
        ResponseNamespace = "urn:com.endeca.service.dataindexing")]
    public void clearContent(string[] in0)
    {
        Invoke("clearContent", new object[]
                                   {
                                       in0
                                   });
    }

    /// <remarks />
    public IAsyncResult BeginclearContent(string[] in0, AsyncCallback callback, object asyncState)
    {
        return BeginInvoke("clearContent", new object[]
                                               {
                                                   in0
                                               }, callback, asyncState);
    }

    /// <remarks />
    public void EndclearContent(IAsyncResult asyncResult)
    {
        EndInvoke(asyncResult);
    }

    /// <remarks />
    [SoapRpcMethod("", RequestNamespace = "urn:com.endeca.service.dataindexing",
        ResponseNamespace = "urn:com.endeca.service.dataindexing")]
    public void startPartialUpdate()
    {
        Invoke("startPartialUpdate", new object[0]);
    }

    /// <remarks />
    public IAsyncResult BeginstartPartialUpdate(AsyncCallback callback, object asyncState)
    {
        return BeginInvoke("startPartialUpdate", new object[0], callback, asyncState);
    }

    /// <remarks />
    public void EndstartPartialUpdate(IAsyncResult asyncResult)
    {
        EndInvoke(asyncResult);
    }

    /// <remarks />
    [SoapRpcMethod("", RequestNamespace = "urn:com.endeca.service.dataindexing",
        ResponseNamespace = "urn:com.endeca.service.dataindexing")]
    public void stopBaselineUpdate()
    {
        Invoke("stopBaselineUpdate", new object[0]);
    }

    /// <remarks />
    public IAsyncResult BeginstopBaselineUpdate(AsyncCallback callback, object asyncState)
    {
        return BeginInvoke("stopBaselineUpdate", new object[0], callback, asyncState);
    }

    /// <remarks />
    public void EndstopBaselineUpdate(IAsyncResult asyncResult)
    {
        EndInvoke(asyncResult);
    }

    /// <remarks />
    [SoapRpcMethod("", RequestNamespace = "urn:com.endeca.service.dataindexing",
        ResponseNamespace = "urn:com.endeca.service.dataindexing")]
    public void startBaselineUpdate()
    {
        Invoke("startBaselineUpdate", new object[0]);
    }

    /// <remarks />
    public IAsyncResult BeginstartBaselineUpdate(AsyncCallback callback, object asyncState)
    {
        return BeginInvoke("startBaselineUpdate", new object[0], callback, asyncState);
    }

    /// <remarks />
    public void EndstartBaselineUpdate(IAsyncResult asyncResult)
    {
        EndInvoke(asyncResult);
    }
}

/// <remarks />
[SoapType("Status", "urn:com.endeca.service.dataindexing")]
public class Status
{
    /// <remarks />
    public SystemError[] systemErrors;

    /// <remarks />
    public string systemState;
}

/// <remarks />
[SoapType("SystemError", "urn:com.endeca.service.dataindexing")]
public class SystemError
{
    /// <remarks />
    public string component;

    /// <remarks />
    public string errorMsg;

    /// <remarks />
    public string recordSpec;

    /// <remarks />
    public string severity;
}

/// <remarks />
[SoapType("PVal", "urn:com.endeca.service.dataindexing")]
public class PVal
{
    /// <remarks />
    public string name;

    /// <remarks />
    public string value;
}

/// <remarks />
[SoapType("Record", "urn:com.endeca.service.dataindexing")]
public class Record
{
    /// <remarks />
    public PVal[] values;
}