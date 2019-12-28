import { Injectable } from '@angular/core';
import { UtilService } from './util.service';
import { BaseHttpService } from './base-http.service';
import { Observable } from 'rxjs';
import { AppLog } from '../Models/app-log';

@Injectable({
  providedIn: 'root'
})
export class DesktopService {

  constructor(
    private util: UtilService,
    private baseHttpService: BaseHttpService
  ) { }

  public SwitchLeft(force: boolean): Observable<AppLog[]> {
    if (this.util.isNullOrUndefined(force)) {
      force = false;
    }

    return this.baseHttpService.post<AppLog[]>("api/Desktop/SwitchLeft?force=" + force);
  }

  public SwitchRight(force: boolean): Observable<AppLog[]> {
    if (this.util.isNullOrUndefined(force)) {
      force = false;
    }

    return this.baseHttpService.post<AppLog[]>("api/Desktop/SwitchRight?force=" + force);
  }
}
