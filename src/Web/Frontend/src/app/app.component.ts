import { Component, OnInit } from '@angular/core';
import { NavigationService } from './Shared/Services/navigation.service';
import { ColorConfiguration } from './Shared/Models/color-configuration';
import { ConfigurationService } from './Shared/Services/configuration.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {

  showLeftNavigationBar: boolean;
  showTopNavigationBar: boolean;

  colorConfiguration: ColorConfiguration;

  constructor(
    private navigationService: NavigationService,
    private configurationService: ConfigurationService
  ) {
  }

  ngOnInit(): void {

    this.navigationService.showLeftNavigationBar.subscribe(
      result => this.showLeftNavigationBar = result
    );

    this.navigationService.showTopNavigationBar.subscribe(
      result => this.showTopNavigationBar = result
    );

    this.configurationService.colorConfiguration.subscribe(
      result => this.colorConfiguration = result
    );

  }
}
