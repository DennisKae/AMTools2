import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { ColorConfiguration } from '../Models/color-configuration';
import { Constants } from '../Models/constants';
import { BaseHttpService } from './base-http.service';

@Injectable({
  providedIn: 'root'
})
export class ConfigurationService {

  colorConfiguration: BehaviorSubject<ColorConfiguration> = new BehaviorSubject(Constants.DEFAULT_COLOR_CONFIGURATION);

  constructor(
    private baseHttpService: BaseHttpService
  ) {

    // baseHttpService.get<ColorConfiguration>("api/Settings/ColorConfiguration").subscribe(
    //   result => this.colorConfiguration.next(result)
    // );

  }
}
