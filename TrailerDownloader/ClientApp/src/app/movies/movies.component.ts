import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Movie } from '../models/movie';
import { MovieService } from '../services/movie.service';
import { SignalrService } from '../services/signalr.service';

@Component({
  selector: 'app-movies',
  templateUrl: './movies.component.html',
  styleUrls: ['./movies.component.scss']
})
export class MoviesComponent implements OnInit {

  allMoviesLoaded: boolean = false;

  constructor(private toastr: ToastrService,
              public signalrService: SignalrService,
              private movieService: MovieService) { }

  ngOnInit(): void {
    this.signalrService.startConnection().then(() => {
      this.getAllMovieInfo();
    });
  }

  private getAllMovieInfo() {
    console.log(this.allMoviesLoaded);
    this.movieService.getAllMovieInfo().subscribe(
      (res: Movie[]) => {
        this.allMoviesLoaded = true;
        this.signalrService.movieList = res;
        this.signalrService.movieList.sort((a, b) => a.title.localeCompare(b.title));
        this.toastr.success(`Retrieved info for ${this.signalrService.movieList.length} movies in your library`, 'Success!');
        console.log(this.signalrService.movieList);
      },
      err => {
        console.log(err);
      }
    )
  }

  downloadAllTrailers(movies: Movie[]) {
    this.toastr.success('Starting download of all trailers');
    this.movieService.downloadAllTrailers(movies).subscribe(
      res => {
        this.toastr.success('Done downloading all trailers', 'Success!');
        console.log('Done downloading trailers');
      },
      err => {
        console.log(err);
      }
    );
  }

  deleteAllTrailers(movies: Movie[]) {
    this.movieService.deleteAllTrailer(movies).subscribe(
      res => {
        this.toastr.success('Done deleting all trailers', 'Success!');
      },
      err => {
        console.log(err);
      }
    )
  }

}
