import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { MovieGuard } from './guards/movie.guard';
import { MoviesComponent } from './movies/movies.component';
import { SetupComponent } from './setup/setup.component';

const routes: Routes = [
  { path: '', redirectTo: 'movies', pathMatch: 'full' },
  { path: 'setup', component: SetupComponent },
  { path: 'movies', component: MoviesComponent, canActivate: [MovieGuard] }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
