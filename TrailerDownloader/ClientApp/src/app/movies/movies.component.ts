import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Movie } from '../models/movie';
import { MovieService } from "../services/movie.service";
import { SignalrService } from '../services/signalr.service';

@Component({
  selector: 'app-movies',
  templateUrl: './movies.component.html',
  styleUrls: ['./movies.component.scss']
})
export class MoviesComponent implements OnInit {

  movieList: Array<Movie> = [];

  constructor(private movieService: MovieService,
              private toastr: ToastrService,
              private signalrService: SignalrService) {
                signalrService.moviesComponent = this;
              }

  ngOnInit(): void {
    this.movieService.getAllMovies().subscribe(res => {
      console.log(res);
      this.movieList = res as Array<Movie>;
    }, err => console.log(err));

    this.signalrService.startConnection();
    this.signalrService.downloadAllTrailersListener();
    this.signalrService.deleteAllTrailersListener();
  }

  downloadAllTrailers(movieList: Array<Movie>) {
    this.toastr.success('Starting download of all trailers', 'Success!');
    this.signalrService.downloadAllTrailers(movieList);
  }

  deleteAllTrailers(movieList: Array<Movie>) {
    this.toastr.warning('Deleting all trailers');
    this.signalrService.deleteAllTrailers(movieList);
  }

}
