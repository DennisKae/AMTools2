import { Injectable } from '@angular/core';
import { ToastService } from './toast.service';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  constructor(
    private toastService: ToastService
  ) { }

  public info(message: string, title?: string): void {
    this.toastService.info(message, title);
  }

  public success(message: string, title?: string): void {
    this.toastService.success(message, title);
  }

  public error(message: string, title?: string): void {
    this.toastService.error(message, title);
  }

}
