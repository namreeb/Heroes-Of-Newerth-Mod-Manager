namespace HON_Mod_Manager.Enums
{
    internal enum ModUpdateStatus
    {
        NotBegun,
        Checking, //querying UpdateCheckURL
        Downloading, //querying UpdateDownloadURL
        NoUpdateInformation, //UpdateCheck is the empty string
        NoUpdatePresent, //UpdateCheckURL did not return a higher version number
        UpdateDownloaded, //successfully downloaded an update
        Failed, //something went wrong
        Aborted //we were asked to stop
    }
}