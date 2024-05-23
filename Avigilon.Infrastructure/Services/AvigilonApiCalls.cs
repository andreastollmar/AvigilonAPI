﻿namespace Avigilon.Infrastructure.Services;
public class AvigilonApiCalls(IHttpClientProvider httpClientProivider, IHandleMediaResponse handleMediaResponse) : IAvigilonApiCalls
{
    public readonly string clientName = "WebEndpointClient";
    public readonly string baseUri = "https://srv03367:8443/mt/api/rest/v1/";
    public readonly string vd1 = "4xIx1DMwMLSwMDUxsTRJStRLTsw1MBASyP2n63D3r8-t3Qtf2Uer7GkBAA";
    public readonly string vd2 = "4xIx1DMwMLSwMDUxsTRJStRLTsw1MBQSyP2n63D3r8-t3Qtf2Uer7GkBAA";
    public readonly string lrf = "4xIx1DMwMLSwMDVMMk9OMdFLTsw1MBASkIsQPdOy12nPRp7ZvgcfBs0CAA";

    private readonly IHttpClientProvider _httpClientProvider = httpClientProivider;
    private readonly IHandleMediaResponse _handleMediaResponse = handleMediaResponse;

    public async Task<List<CameraTimeline>> GetListOfTimelines(string session, RequestBodyContract bodyContract)
    {
        var cameraId = bodyContract.Camera.ToLower().Equals("vd1") ? vd1 : bodyContract.Camera.ToLower().Equals("vd2") ? vd2 : lrf;

        var httpClient = _httpClientProvider.GetHttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", "x-avg-session " + session);

        Uri requestUri = new Uri(baseUri + $"timeline?session={session}&cameraIds={cameraId}&scope={bodyContract.Interval}_SECONDS&start={bodyContract.StartDate}T06:00:00.0&end{bodyContract.EndDate}T06:00:00.0&storage=ALL");

        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(requestUri);
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to get list of entrys: response status code was {httpResponseMessage.StatusCode}");
        }

        var responseString = await httpResponseMessage.Content.ReadAsStringAsync();
        var message = JsonSerializer.Deserialize<TimeLineResult>(responseString);
        return message.Result.Timelines;
    }

    public async Task<bool> SaveMediafile(string session, string time, string date, string camera, bool isImg)
    {
        var httpClient = _httpClientProvider.GetHttpClient();
        httpClient.DefaultRequestHeaders.Add("Authorization", "x-avg-session " + session);
        var dateTime = "live";
        if (!time.Equals("live"))
        {
            dateTime = date + "T" + time + ".057Z,c";
        }

        var content = (new MediaContract
        {
            Session = session,
            CameraId = camera.ToLower().Equals("vd1") ? vd1 : camera.ToLower().Equals("vd2") ? vd2 : lrf,
            Format = isImg ? "jpeg" : "fmp4",
            Media = "video",
            Range = "F300",
            Quality = "low",
            Burnin = false,
            Size = "640,480,le",
            T = dateTime
        });
        Uri requestUri = new Uri(baseUri + $"media?session={content.Session}&cameraId={content.CameraId}&format={content.Format}&t={content.T}");

        HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(requestUri);
        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to get video: response status code was {httpResponseMessage.StatusCode}");
        }
        var mediaData = await httpResponseMessage.Content.ReadAsByteArrayAsync();
        var isSuccess = await _handleMediaResponse.SaveMediaResponse(camera, mediaData, isImg);

        return isSuccess;
    }

}
