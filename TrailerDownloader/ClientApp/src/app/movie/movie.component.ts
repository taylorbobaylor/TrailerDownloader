import { Component, Input, OnInit } from '@angular/core';
import { Movie } from '../models/movie';
import { SignalrService } from '../services/signalr.service';

@Component({
  selector: 'app-movie',
  templateUrl: './movie.component.html',
  styleUrls: ['./movie.component.scss']
})
export class MovieComponent implements OnInit {

  @Input() movieInfo?: Movie;

  constructor(private signalrService: SignalrService) { }

  ngOnInit(): void {
  }

  addTrailerToDownloadArray(movie: Movie) {
    if (!this.signalrService.trailersToDownload.some(item => item.filePath === movie.filePath)) {
      this.signalrService.trailersToDownload.push(movie);
    }
    else {
      this.signalrService.trailersToDownload = this.signalrService.trailersToDownload.filter(item => item.filePath !== movie.filePath);
    }
  }

}
