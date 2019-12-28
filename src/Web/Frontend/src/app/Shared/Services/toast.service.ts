import { Injectable } from '@angular/core';
import { ToastrService, IndividualConfig } from 'ngx-toastr';

@Injectable({
  providedIn: 'root'
})
export class ToastService {

  constructor(
    private toastr: ToastrService
  ) { }

  info(message: string, title: string = "Information"): void {
    this.toastr.info(message, title, {
      closeButton: true
    });
  }

  success(message: string, title: string = "Vorgang erfolgreich"): void {
    this.toastr.success(message, title, {
      closeButton: true
    });
  }

  error(message: string, title: string = "Fehler"): void {
    this.toastr.error(message, title, {
      closeButton: true
    });
  }
}
