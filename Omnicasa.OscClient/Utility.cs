using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using HttpMultipartParser;

// ReSharper disable once IdentifierTypo
namespace Omnicasa.OscClient
{
    /// <inheritdoc/>
    public static class Utility
    {
        /// <summary>
        /// Get picture.
        /// </summary>
        /// <param name="camera">Camera instance.</param>
        /// <param name="sessionId"></param>
        /// <param name="process"></param>
        /// <param name="cancellationToken"></param>
        /// <exception cref="ErrorException"></exception>
        /// <returns></returns>
        /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="ArgumentOutOfRangeException">The <paramref>millisecondsDelay
        ///     <name>millisecondsDelay</name>
        /// </paramref> argument is less than -1.</exception>
        /// <exception cref="ObjectDisposedException">The provided <paramref name="cancellationToken">cancellationToken</paramref> has already been disposed.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        public static async Task<string> GetPictureAsync(
            this Camera camera,
            string sessionId,
            IProgress<float> process = null,
            CancellationToken cancellationToken = default)
        {
            var result = await camera.TakePictureAsync(sessionId, cancellationToken).ConfigureAwait(false);
            switch (result.State)
            {
                case ExecutionState.Error:
                    throw new ErrorException(result.Error.Message, result.Error.Code);
                case ExecutionState.Done:
                    return result.Results.FileUri;
            }

            await Task.Delay(50, cancellationToken).ConfigureAwait(false);
            process?.Report(result.Progress.Completion);
            var id = result.Id;
            do
            {
                var statusResult = await camera.GetStatusAsync(id, cancellationToken).ConfigureAwait(false);
                id = statusResult.Id;
                switch (statusResult.State)
                {
                    case ExecutionState.Done:
                        return statusResult.Results["fileUri"].ToString();
                    case ExecutionState.Error:
                        throw new ErrorException(statusResult.Error.Message, result.Error.Code);
                }

                process?.Report(statusResult.Progress.Completion);
                await Task.Delay(50, cancellationToken).ConfigureAwait(false);
            }
            while (true);
        }

        /// <summary>
        /// Get picture from live preview.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="imageProcessProgress"></param>
        /// <param name="imagePreviewProgress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="OperationCanceledException">The token has had cancellation requested.</exception>
        /// <exception cref="ErrorException">Condition.</exception>
        /// <exception cref="ObjectDisposedException">The associated <see cref="T:System.Threading.CancellationTokenSource"></see> has been disposed.</exception>
        /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static async Task<string> GetLivePreviewPictureAsync(
            this CameraSession session,
            IProgress<byte[]> imagePreviewProgress = null,
            IProgress<float> imageProcessProgress = null,
            CancellationToken cancellationToken = default)
        {
            var flag = false;
            cancellationToken.ThrowIfCancellationRequested();
            var lastStatus = await session.Camera.GetStateAsync(cancellationToken).ConfigureAwait(false);
            var lastFile = lastStatus.State.LatestFileUri;
            do
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    var result = await session.Camera.GetStateAsync(cancellationToken).ConfigureAwait(false);
                    cancellationToken.ThrowIfCancellationRequested();
                    if (lastFile != result.State.LatestFileUri)
                    {
                        return result.State.LatestFileUri;
                    }

                    if (flag)
                    {
                        imageProcessProgress?.Report(0.0f);
                    }

                    lastFile = result.State.LatestFileUri;
                    cancellationToken.ThrowIfCancellationRequested();
                    await session.GetLivePreviewAsync(imagePreviewProgress, cancellationToken).ConfigureAwait(false);
                }
                catch (MultipartParseException)
                {
                    flag = true;
                }
                catch (ErrorException ex)
                {
                    if (ex.Code != "serviceUnavailable")
                    {
                        throw;
                    }
                }
            }
            while (true);
        }

        /// <summary>
        /// Get picture from live preview.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="imageProcessProgress"></param>
        /// <param name="imagePreviewProgress"></param>
        /// <param name="imageProgress"></param>
        /// <param name="resumeImageProgress"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="ErrorException">Condition.</exception>
        /// <exception cref="TaskCanceledException">The task has been canceled.</exception>
        /// <exception cref="HttpRequestException">The request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout.</exception>
        /// <exception cref="Exception">A delegate callback throws an exception.</exception>
        public static async Task TriggerLivePreviewPictureAsync(
            this CameraSession session,
            IProgress<byte[]> imagePreviewProgress = null,
            IProgress<float> imageProcessProgress = null,
            IProgress<string> imageProgress = null,
            bool resumeImageProgress = false,
            CancellationToken cancellationToken = default)
        {
            var flag = false;
            var preview = false;
            var lastStatus = await session.Camera.GetStateAsync(cancellationToken).ConfigureAwait(false);
            var lastFile = lastStatus.State.LatestFileUri;
            do
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                try
                {
                    var result = await session.Camera.GetStateAsync(cancellationToken).ConfigureAwait(false);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    if (lastFile != result.State.LatestFileUri)
                    {
                        flag = false;
                        if (resumeImageProgress)
                        {
                            imageProgress?.Report(result.State.LatestFileUri);
                        }
                        else if (preview)
                        {
                            imageProgress?.Report(result.State.LatestFileUri);
                        }
                    }
                    else
                    {
                        if (resumeImageProgress)
                        {
                            if (flag)
                            {
                                imageProcessProgress?.Report(0.0f);
                            }
                        }
                        else
                        {
                            if (preview == false)
                            {
                                imageProcessProgress?.Report(-1.0f);
                            }
                            else if (flag)
                            {
                                imageProcessProgress?.Report(0.0f);
                            }
                        }
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }

                    lastFile = result.State.LatestFileUri;
                    if (resumeImageProgress)
                    {
                        await session.GetLivePreviewAsync(imagePreviewProgress, cancellationToken).ConfigureAwait(false);
                    }
                    else
                    {
                        await session.GetLivePreviewAsync(
                            new Progress<byte[]>(
                            bytes =>
                        {
                            preview = true;
                            imagePreviewProgress?.Report(bytes);
                        }),
                            cancellationToken).ConfigureAwait(false);
                    }
                }
                catch (MultipartParseException)
                {
                    flag = true;
                }
                catch (ErrorException ex)
                {
                    if (ex.Code != "serviceUnavailable")
                    {
                        throw;
                    }
                }
            }
            while (true);
        }
    }
}