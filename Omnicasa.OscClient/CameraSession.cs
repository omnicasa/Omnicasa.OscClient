using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once IdentifierTypo
namespace Omnicasa.OscClient
{
    /// <summary>
    /// CameraSession
    /// </summary>
    public class CameraSession : IDisposable
    {
        private bool destroyCamera;

        /// <summary>
        /// Gets session id.
        /// </summary>
        public string SessionId { get; }

        /// <summary>
        /// Get session timeout.
        /// </summary>
        public int SessionTimeout { get; }

        /// <summary>
        /// Get camera.
        /// </summary>
        public Camera Camera { get; }

        private CameraSession(Camera camera, string sessionId, int sessionTimeout)
        {
            SessionId = sessionId;
            SessionTimeout = sessionTimeout;
            Camera = camera;
        }

        /// <summary>
        /// Create session from camera.
        /// </summary>
        /// <param name="camera"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="ErrorException">Condition.</exception>
        /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="ArgumentNullException">The <paramref>function
        ///     <name>function</name>
        /// </paramref> parameter is null.</exception>
        public static async Task<CameraSession> CreateAsync(Camera camera, CancellationToken cancellationToken = default)
        {
            var result = await camera.StartSessionAsync(cancellationToken).ConfigureAwait(false);
            if (result.State == ExecutionState.Error)
            {
                throw new ErrorException(result.Error.Message, result.Error.Code);
            }

            return new CameraSession(camera, result.Results.SessionId, result.Results.Timeout);
        }

        /// <summary>
        /// Create session from camera.
        /// </summary>
        /// <param name="port"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="ErrorException">Condition.</exception>
        /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="ArgumentNullException">The <paramref>function
        ///     <name>function</name>
        /// </paramref> parameter is null.</exception>
        public static async Task<CameraSession> CreateAsync(string host = "192.168.1.1", int port = 80, CancellationToken cancellationToken = default)
        {
            var result = await CreateAsync(new Camera { Host = host, Port = port }, cancellationToken).ConfigureAwait(false);
            result.destroyCamera = true;
            return result;
        }

        /// <summary>
        /// Get picture.
        /// </summary>
        /// <param name="process"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ErrorException">Condition.</exception>
        /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="ObjectDisposedException">The provided <paramref name="cancellationToken">cancellationToken</paramref> has already been disposed.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref>millisecondsDelay
        ///     <name>millisecondsDelay</name>
        /// </paramref> argument is less than -1.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        public Task<string> GetPictureAsync(IProgress<float> process = null, CancellationToken cancellationToken = default) => Camera?.GetPictureAsync(SessionId, process, cancellationToken);

        /// <summary>
        /// Get live preview picture.
        /// </summary>
        /// <param name="imageProgress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public Task GetLivePreviewAsync(IProgress<byte[]> imageProgress, CancellationToken cancellationToken = default) => Camera?.GetLivePreviewAsync(SessionId, imageProgress, cancellationToken);

        /// <inheritdoc />
        public async void Dispose()
        {
            try
            {
                await Camera.CloseSessionAsync(SessionId).ConfigureAwait(false);
                if (destroyCamera)
                {
                    Camera?.Dispose();
                }
            }
            catch
            {
                // Ignore
            }
        }
    }
}