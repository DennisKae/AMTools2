import { Injectable } from '@angular/core';
import { BaseHttpService } from './base-http.service';
import { Observable } from 'rxjs';
import { SubscriberViewModel } from '../Models/subscriber-view-model';

@Injectable({
  providedIn: 'root'
})
export class SubscriberService {

  constructor(
    private baseHttpService: BaseHttpService
  ) { }

  public GetAll(): Observable<SubscriberViewModel[]>{
    return this.baseHttpService.get<SubscriberViewModel[]>("api/Subscriber");
  }
}
