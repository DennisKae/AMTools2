import { Component, OnInit } from '@angular/core';
import { ColorConfiguration } from '../../Models/color-configuration';
import { ConfigurationService } from '../../Services/configuration.service';

@Component({
  selector: 'app-not-found-page',
  templateUrl: './not-found-page.component.html',
  styleUrls: ['./not-found-page.component.scss']
})
export class NotFoundPageComponent implements OnInit {

  colorConfiguration: ColorConfiguration;

  constructor(
    private configurationService: ConfigurationService
  ) { }

  ngOnInit() {
    this.configurationService.colorConfiguration.subscribe(result => this.colorConfiguration = result);
  }

}
