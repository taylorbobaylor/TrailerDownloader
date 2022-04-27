import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { ToastrService } from 'ngx-toastr';
import { Movie } from "../models/movie";

@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  hubConnection: signalR.HubConnection;
  movieList: Array<Movie> = [];
  trailersToDownload: Array<Movie> = [];

  allMoviesLoaded: boolean = false;

  constructor(private toastr: ToastrService) { }

  startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
                              .withUrl(window.location.origin + '/moviehub')
                              .build();

    this.hubConnection.start().then(() => {
      console.log('Connection started');
      this.completedAllMoviesInfoListener();
      this.getAllMoviesInfoListener();
      this.doneDownloadingAllTrailersListener();
      this.getAllMoviesInfo();
    }).catch(err => {
      console.log('Error starting connection: ' + err);
    });
  }

  downloadAllTrailersListener = () => {
    this.hubConnection.on('downloadAllTrailers', (data: Movie) => {
      if (data.trailerExists) {
        let indexOfMovieInList = this.movieList.findIndex(x => x.title === data.title);
        this.movieList[indexOfMovieInList] = data;
        this.toastr.success(`Done downloading trailer for ${data.title}`, 'Success!');
      }
      else {
        this.toastr.error(`Issues downloading trailer for ${data.title}, please check the logs`, 'Error');
      }
    });
  }

  downloadAllTrailers(movieList: Array<Movie>) {
    this.hubConnection.invoke('downloadAllTrailers', movieList).catch(err => console.log(err));
  }

  deleteAllTrailersListener = () => {
    this.hubConnection.on('deleteAllTrailers', (data: Movie) => {
      let indexOfMovieInList = this.movieList.findIndex(x => x.title === data.title);
      this.movieList[indexOfMovieInList] = data;
    });
  }

  deleteAllTrailers(movieList: Array<Movie>) {
    this.hubConnection.invoke('deleteAllTrailers', movieList);
  }

  private getAllMoviesInfoListener = () => {
    this.hubConnection.on('getAllMoviesInfo', (data: Movie) => {
      this.movieList.push(data);
      this.movieList.sort((a, b) => a.title.localeCompare(b.title));
    });
  }

  private getAllMoviesInfo() {
    this.hubConnection.invoke('getAllMoviesInfo').catch(err => console.log(err));
  }

  private completedAllMoviesInfoListener = () => {
    this.hubConnection.on('completedAllMoviesInfo', data => {
      console.log(`Retrieved info for ${data} movies in your library`);
      this.toastr.success(`Retrieved info for ${data} movies in your library`, 'Success!');
      this.allMoviesLoaded = true;
    });
  }

  private doneDownloadingAllTrailersListener = () => {
    this.hubConnection.on('doneDownloadingAllTrailersListener', data => {
      if (data === true) {
        this.trailersToDownload = [];
        console.log('Successfully downloaded all missing trailers!');
        this.toastr.success('Done downloading all missing trailers', 'Success!');
      }
    });
  }

}
