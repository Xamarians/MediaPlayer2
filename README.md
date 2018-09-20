# MediaPlayer2
Cross Platform Media Player to play videos in Xamarin.Forms. Supports android and iOS platforms.

First install package from nuget using following command -
## Install-Package Xamarians.MediaPlayer

You can integrate video player in you Xamarin Form application using following code:

 Shared Code -
 
```c#
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://xamarin.com/schemas/2014/forms"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:xamarians="clr-namespace:Xamarians.MediaPlayer;assembly=Xamarians.MediaPlayer"
             x:Class="App1.MainPage">

    <xamarians:VideoPlayer x:Name="videoPlayer" AutoPlay="True" ErrorOcurred="VideoPlayer_ErrorAsync" Completed="VideoPlayer_Completed" Mute="True" Prepared="VideoPlayer_Prepared" SeekBarPositionChanged="videoPlayer_SeekBarPositionChanged_1">
            <xamarians:VideoPlayer.Source>
                <x:Array x:Key="stringArray" Type="{x:Type x:String}">
                    <x:String><Video url></x:String>
                </x:Array>
            </xamarians:VideoPlayer.Source>
        </xamarians:VideoPlayer>
</ContentPage>
```

Android - in MainActivity file write below code -
```c#
Xamarians.MediaPlayer.Droid.VideoPlayerRenderer.Init(this);
```

iOS - in AppDelegate file write below code -
```c#
Xamarians.MediaPlayer.iOS.VideoPlayerRenderer.Init();
```
Also add following lines in info.plist to allow app to open url over http.
```c#
<key>NSAppTransportSecurity</key>
<dict>
<key>NSAllowsArbitraryLoads</key>
<true/>
</dict>
```


### For any issue with library please report here 
https://github.com/Xamarians/MediaPlayer2/issues/new
