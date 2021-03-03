import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Movie } from '../models/movie';

@Injectable({
  providedIn: 'root'
})
export class MovieService {

  movieBaseUrl: string = window.location.origin + "/api/movie/";

  constructor(private http: HttpClient) { }

  getAllMovieInfo() {
    return this.http.get<Movie[]>(this.movieBaseUrl + 'info');
  }

  downloadAllTrailers(movies: Movie[]) {
    return this.http.post(this.movieBaseUrl + 'trailer', movies);
  }

  deleteAllTrailer(movies: Movie[]) {
    return this.http.post(this.movieBaseUrl + 'trailer/delete', movies);
  }
}
