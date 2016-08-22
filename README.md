# playerframework

Clone of https://playerframework.codeplex.com to make it compatible with the latest changes introduced in the Win10 anniversay update.

With the anniversady update if you start a live stream using 

    player.source = new Uri("");
    
It'll never get disposed and therefore it won't stop downloading chunks....

The solucion is to use the method SetPlaybackSource introduced in the Mediaplayer with the anniversay update:

    var mediaSource = MediaSource.CreateFromUri(new Uri(LoadUri.Text));
    mediaElement.SetPlaybackSource(mediaSource);

But because this is not supported in the original player framework, I've updated it to support it:

    var mediaSource = MediaSource.CreateFromUri(new Uri("http://demo-ss-vod.zahs.tv/exodus.ism/Manifest"));
    player.SetMediaStreamSource(mediaSource);

How to build it:
run: BuildSDKs.Win10.bat
