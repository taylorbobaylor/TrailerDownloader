import { Injectable } from '@angular/core';
import * as signalR from "@aspnet/signalr";
import { ToastrService } from 'ngx-toastr';
import { Movie } from "../models/movie";

@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  hubConnection: signalR.HubConnection;
  movieList: Array<Movie> = [];

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
    this.hubConnection.on('downloadAllTrailers', data => {
      this.movieList = data as Array<Movie>;
    });
  }

  downloadAllTrailers(movieList: Array<Movie>) {
    console.log(this.hubConnection.state);

    if (this.hubConnection.state !== 1) {
      this.hubConnection.start().then(() => {
        this.hubConnection.invoke('downloadAllTrailers', movieList).catch(err => console.log(err));
      });
    }
    else {
      this.hubConnection.invoke('downloadAllTrailers', movieList).catch(err => console.log(err));
    }
  }

  deleteAllTrailersListener = () => {
    this.hubConnection.on('deleteAllTrailers', data => {
      this.movieList = data as Array<Movie>;
    });
  }

  deleteAllTrailers(movieList: Array<Movie>) {
    this.hubConnection.invoke('deleteAllTrailers', movieList);
  }

  private getAllMoviesInfoListener = () => {
    this.hubConnection.on('getAllMoviesInfo', data => {
      this.movieList = data as Array<Movie>;
    });
  }

  private getAllMoviesInfo() {
    this.hubConnection.invoke('getAllMoviesInfo').catch(err => console.log(err));
  }

  private completedAllMoviesInfoListener = () => {
    this.hubConnection.on('completedAllMoviesInfo', data => {
      console.log(`Retrieved info for ${data} movies in your library`);
      this.toastr.success(`Retrieved info for ${data} movies in your library`, 'Success!');
    });
  }

  private doneDownloadingAllTrailersListener = () => {
    this.hubConnection.on('doneDownloadingAllTrailersListener', data => {
      if (data === true) {
        console.log('Successfully downloaded all missing trailers!');
        this.toastr.success('Done downloading all missing trailers', 'Success!');
      }
    });
  }

}
