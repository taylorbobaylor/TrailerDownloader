import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { MoviesComponent } from './movies/movies.component';
import { SetupComponent } from './setup/setup.component';
import { PageGuardGuard } from './guard/page-guard.guard';


const routes: Routes = [
  {
    path: '',
    redirectTo: 'movies',
    pathMatch: 'full'
  },
  {
    path: 'setup',
    component: SetupComponent
  },
  {
    path: 'movies',
    component: MoviesComponent,
    canActivate: [PageGuardGuard]
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
