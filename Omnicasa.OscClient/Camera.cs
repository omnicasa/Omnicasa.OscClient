using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HttpMultipartParser;
using Newtonsoft.Json;

// ReSharper disable once IdentifierTypo
namespace Omnicasa.OscClient
{
    /// <inheritdoc />
    public class Camera : IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly JsonSerializer serializer;
        private readonly bool destroyHttpClient;

        /// <summary>
        /// Gets or sets host.
        /// </summary>
        public string Host { get; set; } = "192.168.1.1";

        /// <summary>
        /// Gets or set port.
        /// </summary>
        public int Port { get; set; } = 80;

        /// <summary>
        /// Gets command url.
        /// </summary>
        public string CommandUrl => $"http://{Host}:{Port}/osc/commands/execute";

        /// <summary>
        /// Gets status url.
        /// </summary>
        public string StatusUrl => $"http://{Host}:{Port}/osc/commands/status";

        /// <summary>
        /// Gets state url.
        /// </summary>
        public string StateUrl => $"http://{Host}:{Port}/osc/state";

        /// <summary>
        /// Gets status url.
        /// </summary>
        public string InfoUrl => $"http://{Host}:{Port}/osc/info";

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// Construct camera with http client & serializer.
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="serializer"></param>
        public Camera(HttpClient httpClient, JsonSerializer serializer)
        {
            this.httpClient = httpClient;
            this.httpClient.DefaultRequestHeaders.ConnectionClose = true;
            this.serializer = serializer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// Construct camera with http client & serializer.
        /// </summary>
        /// <param name="httpClient"></param>
        public Camera(HttpClient httpClient)
            : this(httpClient, new JsonSerializer())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// Construct camera with http client & serializer.
        /// </summary>
        /// <param name="serializer"></param>
        public Camera(JsonSerializer serializer)
            : this(new HttpClient(), serializer)
        {
            destroyHttpClient = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Camera"/> class.
        /// Construct camera with http client & serializer.
        /// </summary>
        public Camera()
            : this(new HttpClient(), new JsonSerializer())
        {
            destroyHttpClient = true;
        }

        /// <summary>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="ArgumentNullException">The <paramref>function
        ///     <name>function</name>
        /// </paramref> parameter is null.</exception>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ExecuteResponse<SessionResult>> StartSessionAsync(CancellationToken cancellationToken = default)
        {
            return SendAsync<ExecuteRequest, ExecuteResponse<SessionResult>>(
                new ExecuteRequest
                {
                    Name = "camera.startSession",
                }, CommandUrl,
                cancellationToken);
        }

        /// <summary>
        /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="T:System.Threading.CancellationTokenSource"></see> associated with <paramref name="cancellationToken">cancellationToken</paramref> was disposed.</exception>
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ExecuteResponse> CloseSessionAsync(string sessionId, CancellationToken cancellationToken = default)
        {
            return SendAsync<ExecuteRequest<SessionParameter>, ExecuteResponse>(
                new ExecuteRequest<SessionParameter>
                {
                    Name = "camera.closeSession",
                    Parameters = new SessionParameter { SessionId = sessionId },
                },
                CommandUrl,
                cancellationToken);
        }

        /// <summary>
        /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="version"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ExecuteResponse> SetClientVersionAsync(string sessionId, int version = 2, CancellationToken cancellationToken = default)
        {
            return SendAsync<ExecuteRequest<ClientVersionParameter>, ExecuteResponse>(
                new ExecuteRequest<ClientVersionParameter>
                {
                    Name = "camera.setOptions",
                    Parameters = new ClientVersionParameter
                    {
                        SessionId = sessionId,
                        Options = new Dictionary<string, object> { { "clientVersion", version } },
                    },
                },
                CommandUrl,
                cancellationToken);
        }

        /// <summary>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="ArgumentNullException">The <paramref>requestUri
        ///     <name>requestUri</name>
        /// </paramref> was null.</exception>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<InfoResponse> GetInfoAsync(CancellationToken cancellationToken = default)
        {
            var result = await httpClient.GetStringAsync(InfoUrl).ConfigureAwait(false);
            using var jsonTextReader = new JsonTextReader(new StringReader(result));
            return serializer.Deserialize(jsonTextReader, typeof(InfoResponse)) as InfoResponse;
        }

        /// <summary>
        /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<StateResponse> GetStateAsync(CancellationToken cancellationToken = default)
        {
            return SendAsync<string, StateResponse>(null, StateUrl, cancellationToken);
        }

        /// <summary>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task GetLivePreviewAsync(
            string sessionId,
            CancellationToken cancellationToken = default)
        {
            return SendAsync(
                new ExecuteRequest<SessionParameter>
                {
                    Name = "camera.getLivePreview",
                    Parameters = new SessionParameter { SessionId = sessionId },
                },
                CommandUrl,
                (reader) => Task.CompletedTask,
                cancellationToken);
        }

        /// <summary>
        /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ExecuteResponse<FileUriResult>> TakePictureAsync(
            string sessionId,
            CancellationToken cancellationToken = default)
        {
            return
                SendAsync<ExecuteRequest<SessionParameter>, ExecuteResponse<FileUriResult>>(
                    new ExecuteRequest<SessionParameter>
                    {
                        Name = "camera.takePicture",
                        Parameters = new SessionParameter { SessionId = sessionId },
                    },
                    CommandUrl,
                    cancellationToken);
        }

        /// <summary>
        /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task<ExecuteResponse<IDictionary<string, object>>> GetStatusAsync(string id, CancellationToken cancellationToken = default)
        {
            return SendAsync<ExecuteRequest, ExecuteResponse<IDictionary<string, object>>>(
                new ExecuteRequest { Id = id }, StatusUrl, cancellationToken);
        }

        /// <summary>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="imageProgress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task GetLivePreviewAsync(
            string sessionId,
            IProgress<byte[]> imageProgress,
            CancellationToken cancellationToken = default)
        {
            return SendAsync<ExecuteRequest>(
                    new ExecuteRequest<SessionParameter>
                    {
                        Name = "camera._getLivePreview",
                        Parameters = new SessionParameter { SessionId = sessionId },
                    }, CommandUrl,
                    reader =>
                    {
                        var flag = false;
                        var numberBytes = 0;
                        var totalBytes = 0;
                        byte[] totalBuffer = null;
                        var skip = false;
                        var parser = new StreamingMultipartFormDataParser(reader)
                        {
                            DataHandler = (type, parameters, buffer, bytes) =>
                            {
                                try
                                {
                                    if (flag == false)
                                    {
                                        skip = false;
                                        totalBytes = int.Parse(parameters["content-length"]);
                                        totalBuffer = new byte[totalBytes];
                                        numberBytes = 0;
                                    }

                                    numberBytes += bytes;

                                    if (numberBytes >= totalBytes)
                                    {
                                        Array.Copy(buffer, 0, totalBuffer ?? throw new InvalidOperationException(), numberBytes - bytes, bytes - (numberBytes - totalBytes));
                                        if (!skip)
                                        {
                                            imageProgress?.Report(totalBuffer);
                                        }

                                        flag = false;
                                    }
                                    else
                                    {
                                        Array.Copy(buffer, 0, totalBuffer ?? throw new InvalidOperationException(), numberBytes - bytes, bytes);
                                        flag = true;
                                    }
                                }
                                catch
                                {
                                    skip = true;
                                }
                            },
                        };
                        return parser.RunAsync(cancellationToken);
                    }, cancellationToken);
        }

        /// <summary>
        /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="ArgumentNullException">The <paramref>function
        ///     <name>function</name>
        /// </paramref> parameter is null.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="T:System.Threading.CancellationTokenSource"></see> associated with <paramref name="cancellationToken">cancellationToken</paramref> was disposed.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="ArgumentException"><paramref>stream
        ///     <name>stream</name>
        /// </paramref> does not support reading.</exception>
        /// </summary>
        /// <param name="content"></param>
        /// <param name="uri"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task<TK> SendAsync<T, TK>(T content, string uri, CancellationToken cancellationToken = default)
        {
            using var stringWriter = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
            using var jsonTextWriter = new JsonTextWriter(stringWriter) { Formatting = serializer.Formatting };
            serializer.Serialize(jsonTextWriter, content, typeof(T));

            var payload = stringWriter.ToString();
            var httpContent = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json"),
            };

            using var response = await httpClient.SendAsync(httpContent, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            using var streamData = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            using var streamReader = new StreamReader(streamData);
            using var jsonReader = new JsonTextReader(streamReader);
            return await Task.Run(() => serializer.Deserialize<TK>(jsonReader), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="ArgumentNullException">The <paramref>function
        ///     <name>function</name>
        /// </paramref> parameter was null.</exception>
        /// <exception cref="ObjectDisposedException">The <see cref="T:System.Threading.CancellationTokenSource"></see> associated with <paramref name="cancellationToken">cancellationToken</paramref> was disposed.</exception>
        /// <exception cref="InvalidOperationException">The request message was already sent by the <see cref="T:System.Net.Http.HttpClient"></see> instance.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref>capacity
        ///     <name>capacity</name>
        /// </paramref> is less than zero.</exception>
        /// </summary>
        /// <param name="content"></param>
        /// <param name="uri"></param>
        /// <param name="callback"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected async Task SendAsync<T>(T content, string uri, Func<Stream, Task> callback, CancellationToken cancellationToken = default)
        {
            using var stringWriter = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
            using var jsonTextWriter = new JsonTextWriter(stringWriter) { Formatting = serializer.Formatting };
            serializer.Serialize(jsonTextWriter, content, typeof(T));

            var payload = stringWriter.ToString();
            var httpContent = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json"),
            };

            using var response = await httpClient.SendAsync(httpContent, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
            using var streamData = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            await Task.Run(() => callback(streamData), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            if (destroyHttpClient)
            {
                httpClient?.Dispose();
            }
        }
    }
}
