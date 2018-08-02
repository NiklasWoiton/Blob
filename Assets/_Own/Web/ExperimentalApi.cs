using UnityEngine;
using Unosquare.Labs.EmbedIO;
using Unosquare.Labs.EmbedIO.Modules;
using Unosquare.Labs.EmbedIO.Constants;
using Unosquare.Net;
using System;
using System.Threading;
using System.Linq;

public class ExperimentalApi : WebApiController
{
    private Spawner[] spawners;
    private Semaphore semaphore = new Semaphore(0, 1);

    public ExperimentalApi()
    {
        UnityMainThreadDispatcher.Instance().Enqueue((() =>
        {
            spawners = GameObject.FindObjectsOfType<Spawner>().OrderBy(spawner => spawner.transform.GetSiblingIndex()).ToArray();
            semaphore.Release();
        }));
        semaphore.WaitOne();
    }

    [WebApiHandler(HttpVerbs.Get, "/api/SystemInfo/{propertyName}")]
    public bool GetSystemInfoProperty(WebServer server, HttpListenerContext context, string propertyName)
    {
        ExecuteSynchronouslyInMainThreadAndReturnResultAsJson(context, () => typeof(SystemInfo).GetField(propertyName).GetValue(null));
        return true;
    }

    [WebApiHandler(HttpVerbs.Get, "/api/DroneSpawner/{index}/instances")]
    public bool GetSpawnerInstances(WebServer server, HttpListenerContext context, int index)
    {
        ExecuteSynchronouslyInMainThreadAndReturnResultAsJson(context, () => spawners[index].instances);
        return true;
    }

    [WebApiHandler(HttpVerbs.Get, "/api/DroneSpawner/{index}/maxInstances")]
    public bool GetSpawnerMaxInstances(WebServer server, HttpListenerContext context, int index)
    {
        ExecuteSynchronouslyInMainThreadAndReturnResultAsJson(context, () => spawners[index].maxInstances);
        return true;
    }

    [WebApiHandler(HttpVerbs.Get, "/api/SpeechRecognition/{message}")]
    public bool ShouldBePutSpeechRecognition(WebServer server, HttpListenerContext context, string message)
    {
        ExecuteSynchronouslyInMainThreadAndReturnResultAsJson(context, () => { SpeechRecognition.OnSpeechResult(message); return message; });
        return true;
    }

    [WebApiHandler(HttpVerbs.Put, "/api/DroneSpawner/{index}/maxInstances/{value}")]
    public bool PutSpawnerMaxInstances(WebServer server, HttpListenerContext context, int index, int value)
    {
        ExecuteSynchronouslyInMainThreadAndReturnResultAsJson(context, () => { return spawners[index].maxInstances = value; });
        return true;
    }

    private delegate object FunctionObject();
    private void ExecuteSynchronouslyInMainThreadAndReturnResultAsJson(HttpListenerContext context, FunctionObject function)
    {
        object result = null;
        UnityMainThreadDispatcher.Instance().Enqueue((() =>
        {
            try
            {
                result = function();
            }
            catch (Exception ex)
            {
                result = ex;
            }
            finally
            {
                semaphore.Release();
            }
        }));
        semaphore.WaitOne();

        if (!(result is Exception))
        {
            if (result is string)
            {
                context.JsonResponse((string)result);
            }
            else
            {
                context.JsonResponse(result.ToString());
            }
        }
        else
        {
            context.JsonExceptionResponse((Exception)result);
        }
    }
}

