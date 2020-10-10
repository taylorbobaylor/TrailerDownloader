# TrailerDownloader

## Description
Downloads all trailers for movies in your Plex library

## docker cli
```bash
docker run -d \
  --name=movie-trailer-downloader \
  -p 6767:80 \
  -v /path/to/movies:/movies \
  taylorbobaylor/movie-trailer-downloader
```

[Docker Hub Repo](https://hub.docker.com/repository/docker/taylorbobaylor/movie-trailer-downloader)

## Demo
![til](./TrailerDownloader/Demo/TrailerDownloader.gif)
