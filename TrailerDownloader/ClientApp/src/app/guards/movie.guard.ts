import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { ConfigService } from '../services/config.service';
import { SignalrService } from '../services/signalr.service';

@Injectable({
  providedIn: 'root'
})
export class MovieGuard implements CanActivate {

  constructor(private router: Router, private configService: ConfigService, private signalrService: SignalrService) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

      return this.configService.getConfig().toPromise().then(
        res => {
          return true;
        },
        err => {
          console.log('No config saved so redirecting to setup page.');
          this.router.navigate(['setup']);
          return false;
        }
      );
  }

}
