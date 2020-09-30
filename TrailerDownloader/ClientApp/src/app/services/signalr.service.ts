import { Injectable } from '@angular/core';
import * as signalR from "@aspnet/signalr";
import { Movie } from "../models/movie";
import { MoviesComponent } from '../movies/movies.component';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  hubConnection: signalR.HubConnection;
  moviesComponent: MoviesComponent;

  constructor() { }

  startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
                              .withUrl(window.location.origin + '/moviehub')
                              .build();

    this.hubConnection.start().then(() => {
      console.log('Connection started');
    }).catch(err => {
      console.log('Error starting connection: ' + err);
    });
  }

  downloadAllTrailersListener = () => {
    this.hubConnection.on('downloadAllTrailers', data => {
      console.log(data);
      this.moviesComponent.movieList = data as Array<Movie>;
    });
  }

  downloadAllTrailers(movieList: Array<Movie>) {
    this.hubConnection.invoke('downloadAllTrailers', movieList).catch(err => console.log(err));
  }

  deleteAllTrailersListener = () => {
    this.hubConnection.on('deleteAllTrailers', data => {
      this.moviesComponent.movieList = data as Array<Movie>;
    });
  }

  deleteAllTrailers(movieList: Array<Movie>) {
    this.hubConnection.invoke('deleteAllTrailers', movieList);
  }
}
