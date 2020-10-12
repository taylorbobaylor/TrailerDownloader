# TrailerDownloader

## Description
Downloads all missing trailers for movies in your Plex library. When it downloads, it will place the trailer in the same folder the movie is in with the name {MovieTitle}-trailer.

## Installation

### docker cli
```bash
docker run -d \
  --name=movie-trailer-downloader \
  -p 6767:80 \
  -v /path/to/movies:/movies \
  taylorbobaylor/movie-trailer-downloader
```

[Docker Hub Repo](https://hub.docker.com/repository/docker/taylorbobaylor/movie-trailer-downloader)

### Windows
1. Download the latest TrailerDownloaderWindows.zip [here](https://github.com/taylorbobaylor/TrailerDownloader/releases/download/v1.0.0/TrailerDownloaderWindows.zip)
2. Extract the zip to your preferred directory.
3. Run TrailerDownloader.exe

## Demo
![til](./TrailerDownloader/Demo/TrailerDownloader.gif)

## Structure
This app expects your movies to be in a specific structure. If your movies do not match the format below, you will not be able to use this.

-Movies  
---Movie Title 1 (2014)  
-----Movie Title 1 (2014).mp4  
---Movie Title 2 (2009)  
-----Movie Title 2 (2009).mkv 

## Donate

I did this for fun and a learning experience but feel free to buy me a cup of coffee so I can continue to make this app awesome!

[![paypal](https://www.paypalobjects.com/en_US/i/btn/btn_donateCC_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=ZRP9ZGW3RDDRN)

Hopefully this will help someone and enjoy!
