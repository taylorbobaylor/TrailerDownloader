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

  constructor(private configService: ConfigService, private router: Router,
              private toastr: ToastrService) { }

  ngOnInit(): void {
  }

  onSubmit(form: NgForm) {
    if (form.valid) {
      this.configService.saveConfig(form.value).subscribe(res => {
        if (res === true) {
          this.toastr.success('Configuration saved', 'Success!');
          this.router.navigate(['movies']);
        }
        else {
          console.log('Media directory path does not exist... Please try again.');
          this.toastr.error('Media directory path does not exist... Please try again', 'Error');
        }
      }, err => {
        console.log(err);
      });
    }
  }

}
