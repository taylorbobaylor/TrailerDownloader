import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { NgForm } from '@angular/forms';

@Injectable({
  providedIn: 'root'
})
export class ConfigService {

  configEndpoint: string = window.location.origin + "/api/config";

  constructor(private http: HttpClient) { }

  getConfig() {
    return this.http.get(this.configEndpoint);
  }

  saveConfig(form: NgForm) {
    return this.http.post(this.configEndpoint, form);
  }

}
