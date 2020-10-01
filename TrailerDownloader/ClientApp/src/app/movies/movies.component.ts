import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Movie } from '../models/movie';
import { SignalrService } from '../services/signalr.service';

@Component({
  selector: 'app-movies',
  templateUrl: './movies.component.html',
  styleUrls: ['./movies.component.scss']
})
export class MoviesComponent implements OnInit {

  constructor(private toastr: ToastrService,
              public signalrService: SignalrService) {}

  ngOnInit(): void {
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
