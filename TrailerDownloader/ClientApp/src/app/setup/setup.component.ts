import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { ConfigService } from '../services/config.service';

@Component({
  selector: 'app-setup',
  templateUrl: './setup.component.html',
  styleUrls: ['./setup.component.scss']
})
export class SetupComponent implements OnInit {

  constructor(private configService: ConfigService,
              private toastr: ToastrService,
              private router: Router) { }

  ngOnInit(): void {
  }

  saveConfig(form: NgForm) {
    if (form.valid) {
      this.configService.saveConfig(form.value).subscribe(
        res => {
          if (res === true) {
            this.toastr.success('Configuration saved', 'Success!');
            this.router.navigate(['movies']);
            return;
          }
        },
        err => {
          if (err.status === 404) {
            this.toastr.error('Path does not exist... Please try again', 'Error');
          }
          else if (err.status === 500) {
            this.toastr.error('There was an issue saving the config. Please check the logs.', 'Error');
          }
        }
      )
    }
    else {
      this.toastr.error('Enter a media directory path.', 'Error');
    }
  }

}
