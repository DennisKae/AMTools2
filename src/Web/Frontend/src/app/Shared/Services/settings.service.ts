import { Injectable } from '@angular/core';
import { BaseHttpService } from './base-http.service';
import { Observable } from 'rxjs';
import { QualificationSettingViewModel } from '../Models/Settings/qualification-setting-view-model';
import { GuiVisibilityViewModel } from '../Models/gui-visibility-view-model';

@Injectable({
  providedIn: 'root'
})
export class SettingsService {

  constructor(
    private baseHttpService: BaseHttpService
  ) { }

  public GetAllQualificationSettings(): Observable<QualificationSettingViewModel[]> {
    return this.baseHttpService.get<QualificationSettingViewModel[]>("api/Settings/Qualifications");
  }

  public GetGuiVisibility(): Observable<GuiVisibilityViewModel> {
    return this.baseHttpService.get<GuiVisibilityViewModel>("api/Settings/GuiVisibility");
  }
}
