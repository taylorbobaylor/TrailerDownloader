import { Injectable } from '@angular/core';
import * as signalR from "@microsoft/signalr";
import { ToastrService } from 'ngx-toastr';
import { Movie } from '../models/movie';


@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  trailersToDownload: Array<Movie> = [];
  movieList: Array<Movie> = [];
  hubConnection: signalR.HubConnection = new signalR.HubConnectionBuilder()
                                            .withUrl(window.location.origin + '/moviehub')
                                            .withAutomaticReconnect()
                                            .build();

  constructor(private toastr: ToastrService) { }

  startConnection() {
    return this.hubConnection.start().then(() => {
      console.log('Hub connection started');
      this.receiveMovieInfo();
      this.downloadAllTrailers();
    })
    .catch(err => {
      console.log('Error starting movie hub connection: ' + err);
    });
  }

  private receiveMovieInfo = () => {
    this.hubConnection.on('receiveMovieInfo', (data: Movie) => {
      let indexOfMovieInList = this.movieList.findIndex(x => x.title === data.title);
      if (indexOfMovieInList !== -1) {
        this.movieList[indexOfMovieInList] = data;
      }
      else {
        this.movieList.push(data);
      }
      this.movieList.sort((a, b) => a.title.localeCompare(b.title));
    });
  }

  private downloadAllTrailers = () => {
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

}
